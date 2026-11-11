using MarketHub.BLL.Common;
using MarketHub.BLL.Interfaces;
using MarketHub.BLL.Models;
using MarketHub.DAL.Data;
using MarketHub.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace MarketHub.BLL.Services;


public class ProductService : IProductService
{
    public const int MaxImagesPerProduct = 8;

    private const int MaxNameLength = 150;
    private const int MaxDescriptionLength = 2000;
    private const decimal MaxPrice = 1_000_000m;
    private const int MaxStock = 1_000_000;

    // Real content inspection (magic bytes, size limits) is TASK-58. Here we only reject the obvious.
    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
    private static readonly string[] AllowedContentTypes = { "image/jpeg", "image/png", "image/webp" };

    private readonly ApplicationDbContext _db;
    private readonly IFileStorage _storage;

    public ProductService(ApplicationDbContext db, IFileStorage storage)
    {
        _db = db;
        _storage = storage;
    }

    /// <summary>The ONLY entry point to products in this class: everything is scoped to the vendor.</summary>
    private IQueryable<Product> OwnProducts(int vendorId) =>
        _db.Products.Where(p => p.VendorProfileId == vendorId);

 
    // Read
  

    public async Task<PagedResult<ProductManageDto>> ListAsync(
        int vendorId, string? search, int page, int pageSize, CancellationToken ct = default)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 10 : Math.Min(pageSize, ProductQuery.MaxPageSize);

        var query = OwnProducts(vendorId).AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            if (term.Length > 100)
            {
                term = term[..100];
            }

            // Escape LIKE wildcards so searching for "50%" does not match everything.
            var pattern = "%" + term.Replace("\\", "\\\\").Replace("%", "\\%").Replace("_", "\\_").Replace("[", "\\[") + "%";
            query = query.Where(p => EF.Functions.Like(p.Name, pattern, "\\"));
        }

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(p => p.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new ProductManageDto(
                p.Id, p.Name, p.Price, p.StockQuantity, p.IsActive,
                p.CategoryId, p.Category.Name,
                p.Images
                    .OrderByDescending(i => i.IsPrimary).ThenBy(i => i.SortOrder)
                    .Select(i => i.ImagePath)
                    .FirstOrDefault()))
            .ToListAsync(ct);

        return new PagedResult<ProductManageDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = total
        };
    }

    public async Task<ProductEditDto?> GetForEditAsync(int vendorId, int productId, CancellationToken ct = default)
    {
        var product = await OwnProducts(vendorId)
            .AsNoTracking()
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == productId, ct);

        if (product is null)
        {
            return null;
        }

        var input = new ProductInput
        {
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            CategoryId = product.CategoryId,
            IsActive = product.IsActive,
            RowVersion = Convert.ToBase64String(product.RowVersion)
        };

        var images = product.Images
            .OrderByDescending(i => i.IsPrimary).ThenBy(i => i.SortOrder)
            .Select(i => new ProductImageDto(i.Id, i.ImagePath, i.IsPrimary))
            .ToList();

        return new ProductEditDto(product.Id, input, images);
    }

   
    // Create / Update
    

    public async Task<Result<int>> CreateAsync(
        int vendorId, ProductInput input, IReadOnlyList<UploadedFile> images, CancellationToken ct = default)
    {
        var error = ValidateInput(input) ?? ValidateNewImages(images, existingImageCount: 0);
        if (error is not null)
        {
            return Result<int>.Fail(error);
        }

        if (!await CategoryIsUsableAsync(input.CategoryId, ct))
        {
            return Result<int>.Fail("Please choose a valid category.");
        }

        var product = new Product
        {
            VendorProfileId = vendorId,
            CategoryId = input.CategoryId,
            Name = input.Name.Trim(),
            Description = NormalizeDescription(input.Description),
            Price = input.Price,
            StockQuantity = input.StockQuantity,
            IsActive = input.IsActive
        };

        var savedFiles = new List<string>();
        try
        {
            await AttachImagesAsync(product, images, savedFiles, ct);

            _db.Products.Add(product);
            await _db.SaveChangesAsync(ct);

            return Result<int>.Ok(product.Id, "Product created.");
        }
        catch (InvalidOperationException ex)
        {
            // The file storage refuses a file (type / size / empty). That is a user error, not a crash.
            DeleteFiles(savedFiles);
            return Result<int>.Fail(ex.Message);
        }
        catch
        {
            DeleteFiles(savedFiles);
            throw;
        }
    }

    public async Task<Result> UpdateAsync(
        int vendorId, int productId, ProductInput input, IReadOnlyList<UploadedFile> newImages,
        CancellationToken ct = default)
    {
        var product = await OwnProducts(vendorId)
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == productId, ct);

        if (product is null)
        {
            return Result.NotFoundResult("Product not found.");
        }

        var error = ValidateInput(input) ?? ValidateNewImages(newImages, product.Images.Count);
        if (error is not null)
        {
            return Result.Fail(error);
        }

        if (input.CategoryId != product.CategoryId && !await CategoryIsUsableAsync(input.CategoryId, ct))
        {
            return Result.Fail("Please choose a valid category.");
        }

        // Optimistic concurrency (completed by TASK-35): compare against the version the vendor saw in the form.
        if (!string.IsNullOrWhiteSpace(input.RowVersion))
        {
            try
            {
                _db.Entry(product).Property(p => p.RowVersion).OriginalValue = Convert.FromBase64String(input.RowVersion);
            }
            catch (FormatException)
            {
                return Result.Fail("The form data is not valid. Please reload the page and try again.");
            }
        }

        product.Name = input.Name.Trim();
        product.Description = NormalizeDescription(input.Description);
        product.Price = input.Price;
        product.StockQuantity = input.StockQuantity;
        product.CategoryId = input.CategoryId;
        product.IsActive = input.IsActive;
        product.UpdatedAt = DateTime.UtcNow;

        var savedFiles = new List<string>();
        try
        {
            await AttachImagesAsync(product, newImages, savedFiles, ct);
            await _db.SaveChangesAsync(ct);

            return Result.Ok("Product updated.");
        }
        catch (DbUpdateConcurrencyException)
        {
            DeleteFiles(savedFiles);
            return Result.Fail(ConcurrencyMessage);
        }
        catch (InvalidOperationException ex)
        {
            DeleteFiles(savedFiles);
            return Result.Fail(ex.Message);
        }
        catch
        {
            DeleteFiles(savedFiles);
            throw;
        }
    }


    // Small operations
   

    public async Task<Result> SetActiveAsync(int vendorId, int productId, bool isActive, CancellationToken ct = default)
    {
        var product = await OwnProducts(vendorId).FirstOrDefaultAsync(p => p.Id == productId, ct);
        if (product is null)
        {
            return Result.NotFoundResult("Product not found.");
        }

        product.IsActive = isActive;
        product.UpdatedAt = DateTime.UtcNow;

        return await SaveAsync(isActive ? "The product is now visible." : "The product is now hidden.", ct);
    }

    public async Task<Result> AdjustStockAsync(int vendorId, int productId, int newQuantity, CancellationToken ct = default)
    {
        if (newQuantity < 0 || newQuantity > MaxStock)
        {
            return Result.Fail($"Stock must be between 0 and {MaxStock:N0}.");
        }

        var product = await OwnProducts(vendorId).FirstOrDefaultAsync(p => p.Id == productId, ct);
        if (product is null)
        {
            return Result.NotFoundResult("Product not found.");
        }

        product.StockQuantity = newQuantity;
        product.UpdatedAt = DateTime.UtcNow;

        return await SaveAsync("Stock updated.", ct);
    }

   
    // Images (the Controller/UI part of image management is TASK-34)


    public async Task<Result> DeleteImageAsync(int vendorId, int productId, int imageId, CancellationToken ct = default)
    {
        var product = await OwnProducts(vendorId)
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == productId, ct);

        var image = product?.Images.FirstOrDefault(i => i.Id == imageId);
        if (product is null || image is null)
        {
            return Result.NotFoundResult("Image not found.");
        }

        var path = image.ImagePath;
        var wasPrimary = image.IsPrimary;

        product.Images.Remove(image);
        _db.Set<ProductImage>().Remove(image);

        // The product must never be left without a primary image while it still has images.
        if (wasPrimary)
        {
            var next = product.Images.OrderBy(i => i.SortOrder).FirstOrDefault();
            if (next is not null)
            {
                next.IsPrimary = true;
            }
        }

        var result = await SaveAsync("Image deleted.", ct);
        if (result.Success)
        {
            _storage.Delete(path); // only after the database change really happened
        }

        return result;
    }

    public async Task<Result> SetPrimaryImageAsync(int vendorId, int productId, int imageId, CancellationToken ct = default)
    {
        var product = await OwnProducts(vendorId)
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == productId, ct);

        if (product is null || product.Images.All(i => i.Id != imageId))
        {
            return Result.NotFoundResult("Image not found.");
        }

        foreach (var image in product.Images)
        {
            image.IsPrimary = image.Id == imageId;
        }

        return await SaveAsync("Primary image changed.", ct);
    }

  
    // Delete
  

    public async Task<Result> DeleteAsync(int vendorId, int productId, CancellationToken ct = default)
    {
        var product = await OwnProducts(vendorId)
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == productId, ct);

        if (product is null)
        {
            return Result.NotFoundResult("Product not found.");
        }

        // An ordered product is part of the customers' and vendors' financial history: hide it, never delete it.
        var hasOrders = await _db.OrderItems.AnyAsync(oi => oi.ProductId == productId, ct);
        if (hasOrders)
        {
            product.IsActive = false;
            product.UpdatedAt = DateTime.UtcNow;
            return await SaveAsync("This product has orders, so it was hidden instead of deleted.", ct);
        }

        var paths = product.Images.Select(i => i.ImagePath).ToList();

        // A cart keeps no history: remove the lines explicitly instead of relying on a cascade rule.
        var cartLines = await _db.CartItems.Where(c => c.ProductId == productId).ToListAsync(ct);
        _db.CartItems.RemoveRange(cartLines);
        _db.Set<ProductImage>().RemoveRange(product.Images);
        _db.Products.Remove(product);

        var result = await SaveAsync("Product deleted.", ct);
        if (result.Success)
        {
            DeleteFiles(paths);
        }

        return result;
    }


    // Helpers
  

    private const string ConcurrencyMessage =
        "Someone else changed this product while you were working on it. Reload the page and try again.";

    private async Task<Result> SaveAsync(string successMessage, CancellationToken ct)
    {
        try
        {
            await _db.SaveChangesAsync(ct);
            return Result.Ok(successMessage);
        }
        catch (DbUpdateConcurrencyException)
        {
            return Result.Fail(ConcurrencyMessage);
        }
    }

    private Task<bool> CategoryIsUsableAsync(int categoryId, CancellationToken ct) =>
        _db.Categories.AnyAsync(c => c.Id == categoryId && c.IsActive, ct);

    private static string? NormalizeDescription(string? description) =>
        string.IsNullOrWhiteSpace(description) ? null : description.Trim();

    /// <summary>Business-rule validation. The ViewModel already checked the shape. Returns an error message or null.</summary>
    private static string? ValidateInput(ProductInput input)
    {
        var name = input.Name?.Trim() ?? string.Empty;
        if (name.Length == 0)
        {
            return "Product name is required.";
        }

        if (name.Length > MaxNameLength)
        {
            return $"Product name can not be longer than {MaxNameLength} characters.";
        }

        if (input.Description is { Length: > MaxDescriptionLength })
        {
            return $"Description can not be longer than {MaxDescriptionLength:N0} characters.";
        }

        if (input.Price <= 0 || input.Price > MaxPrice)
        {
            return $"Price must be greater than 0 and not more than {MaxPrice:N0}.";
        }

        if (decimal.Round(input.Price, 2) != input.Price)
        {
            return "Price can have at most 2 decimal places.";
        }

        if (input.StockQuantity < 0 || input.StockQuantity > MaxStock)
        {
            return $"Stock must be between 0 and {MaxStock:N0}.";
        }

        return null;
    }

    private static string? ValidateNewImages(IReadOnlyList<UploadedFile> files, int existingImageCount)
    {
        if (files.Count == 0)
        {
            return null;
        }

        if (existingImageCount + files.Count > MaxImagesPerProduct)
        {
            return $"A product can have at most {MaxImagesPerProduct} images.";
        }

        foreach (var file in files)
        {
            var extension = Path.GetExtension(file.FileName ?? string.Empty).ToLowerInvariant();
            var contentType = (file.ContentType ?? string.Empty).ToLowerInvariant();

            if (!AllowedExtensions.Contains(extension) || !AllowedContentTypes.Contains(contentType))
            {
                return "Only JPG, PNG or WEBP images are allowed.";
            }
        }

        return null;
    }

    /// <summary>Saves the files and adds ProductImage rows. The first image of a product becomes its primary image.</summary>
    private async Task AttachImagesAsync(
        Product product, IReadOnlyList<UploadedFile> files, List<string> savedFiles, CancellationToken ct)
    {
        if (files.Count == 0)
        {
            return;
        }

        var hasPrimary = product.Images.Any(i => i.IsPrimary);
        var sortOrder = product.Images.Count == 0 ? 0 : product.Images.Max(i => i.SortOrder) + 1;

        foreach (var file in files)
        {
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var path = await _storage.SaveAsync(file.Content, extension, ct);
            savedFiles.Add(path);

            product.Images.Add(new ProductImage
            {
                ImagePath = path,
                IsPrimary = !hasPrimary,
                SortOrder = sortOrder++
            });
            hasPrimary = true;
        }
    }

    private void DeleteFiles(IEnumerable<string> paths)
    {
        foreach (var path in paths)
        {
            _storage.Delete(path);
        }
    }
}
