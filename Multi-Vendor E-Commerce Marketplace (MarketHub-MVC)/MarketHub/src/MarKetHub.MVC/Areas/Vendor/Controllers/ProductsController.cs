using MarketHub.BLL.Interfaces;
using MarketHub.BLL.Models;
using MarketHub.MVC.Infrastructure.Dev;
using MarketHub.MVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MarketHub.MVC.Areas.Vendor.Controllers;

/// <summary>
/// TASK-19. The vendor's own products: list, create, edit, delete.
/// Every call to the service passes <see cref="VendorControllerBase.VendorId"/> (from the login), so another vendor's
/// product answers 404 exactly like a product that does not exist.
/// Rules of the team: ModelState check -> call the service -> flash the Result -> redirect (Post-Redirect-Get).
/// Later tasks add to this controller: images (TASK-34), concurrent edit message (TASK-35), search / paging / quick stock (TASK-37).
/// </summary>
public class ProductsController : VendorControllerBase
{
    private const int PageSize = 10;

    // A form may carry up to 8 images of 2 MB each (see ProductService / LocalFileStorage). Everything bigger is refused early.
    private const long MaxRequestBytes = 20 * 1024 * 1024;

    private readonly IProductService _products;
    private readonly IVendorCategoryOptions _categories;

    public ProductsController(
        IProductService products, IVendorCategoryOptions categories, ICurrentVendorService currentVendor)
        : base(currentVendor)
    {
        _products = products;
        _categories = categories;
    }

    // ------------------------------------------------------------------ list

    [HttpGet]
    public async Task<IActionResult> Index(int page = 1, CancellationToken ct = default)
    {
        var result = await _products.ListAsync(VendorId, null, page, PageSize, ct);

        // Deleting the last product of the last page leaves an empty page: go to the last page that exists.
        if (result.Items.Count == 0 && page > 1)
        {
            return RedirectToAction(nameof(Index), new { page = Math.Max(1, result.TotalPages) });
        }

        return View(result);
    }

    // ------------------------------------------------------------------ create

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken ct)
    {
        var model = new ProductFormViewModel();
        await FillCategoriesAsync(model, ct);
        return View(model);
    }

    [HttpPost]
    [RequestSizeLimit(MaxRequestBytes)]
    [RequestFormLimits(MultipartBodyLengthLimit = MaxRequestBytes)]
    public async Task<IActionResult> Create(ProductFormViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            await FillCategoriesAsync(model, ct);
            return View(model);
        }

        using var uploads = OpenUploads(model.NewImages);
        var result = await _products.CreateAsync(VendorId, ToInput(model), uploads.Files, ct);

        if (!result.Success)
        {
            // Business-rule failure (bad category, bad image, ...): show it on the form and keep what the vendor typed.
            ModelState.AddModelError(string.Empty, result.Message);
            await FillCategoriesAsync(model, ct);
            return View(model);
        }

        Flash(result);
        return RedirectToAction(nameof(Index));
    }

    // ------------------------------------------------------------------ edit

    [HttpGet]
    public async Task<IActionResult> Edit([FromRoute] int id, CancellationToken ct)
    {
        var product = await _products.GetForEditAsync(VendorId, id, ct);
        if (product is null)
        {
            return NotFound(); // not yours and not existing look the same
        }

        var input = product.Input;
        var model = new ProductFormViewModel
        {
            Id = product.Id,
            Name = input.Name,
            Description = input.Description,
            Price = input.Price,
            StockQuantity = input.StockQuantity,
            CategoryId = input.CategoryId,
            IsActive = input.IsActive,
            RowVersion = input.RowVersion,
            ExistingImages = product.Images
        };

        await FillCategoriesAsync(model, ct);
        return View(model);
    }

    [HttpPost]
    [RequestSizeLimit(MaxRequestBytes)]
    [RequestFormLimits(MultipartBodyLengthLimit = MaxRequestBytes)]
    public async Task<IActionResult> Edit([FromRoute] int id, ProductFormViewModel model, CancellationToken ct)
    {
        model.Id = id;

        if (!ModelState.IsValid)
        {
            return await ReShowEditAsync(model, ct);
        }

        using var uploads = OpenUploads(model.NewImages);
        var result = await _products.UpdateAsync(VendorId, id, ToInput(model), uploads.Files, ct);

        if (result.NotFound)
        {
            return NotFound();
        }

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return await ReShowEditAsync(model, ct);
        }

        Flash(result);
        return RedirectToAction(nameof(Index));
    }

    // ------------------------------------------------------------------ delete

    /// <summary>
    /// POST only. A product with orders is hidden instead of deleted: the service decides and its message tells the vendor.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Delete([FromRoute] int id, int page = 1, CancellationToken ct = default)
    {
        var result = await _products.DeleteAsync(VendorId, id, ct);
        return RedirectWithResult(result, nameof(Index), new { page });
    }

    // ------------------------------------------------------------------ helpers

    /// <summary>Maps field by field. Nothing the browser sends can reach the service except these fields.</summary>
    private static ProductInput ToInput(ProductFormViewModel model) => new()
    {
        Name = model.Name.Trim(),
        Description = model.Description,
        Price = model.Price!.Value,
        StockQuantity = model.StockQuantity!.Value,
        CategoryId = model.CategoryId!.Value,
        IsActive = model.IsActive,
        RowVersion = model.RowVersion
    };

    private async Task FillCategoriesAsync(ProductFormViewModel model, CancellationToken ct)
    {
        var categories = await _categories.GetActiveAsync(ct);
        model.Categories = categories
            .Select(c => new SelectListItem(c.Name, c.Id.ToString(), c.Id == model.CategoryId))
            .ToList();
    }

    /// <summary>Re-displays the edit form after an error: categories and the saved images are display data, not posted data.</summary>
    private async Task<IActionResult> ReShowEditAsync(ProductFormViewModel model, CancellationToken ct)
    {
        var current = await _products.GetForEditAsync(VendorId, model.Id, ct);
        if (current is null)
        {
            return NotFound();
        }

        model.ExistingImages = current.Images;
        await FillCategoriesAsync(model, ct);
        return View(model);
    }

    /// <summary>
    /// Wraps the posted files in the framework-neutral UploadedFile of the BLL. Empty file inputs are skipped.
    /// The streams are opened here and closed when the caller disposes the returned object.
    /// </summary>
    private static UploadSet OpenUploads(IEnumerable<IFormFile>? files)
    {
        var set = new UploadSet();
        foreach (var file in files ?? Enumerable.Empty<IFormFile>())
        {
            if (file.Length == 0)
            {
                continue;
            }

            var stream = file.OpenReadStream();
            set.Streams.Add(stream);
            set.Files.Add(new UploadedFile(Path.GetFileName(file.FileName), file.ContentType, file.Length, stream));
        }

        return set;
    }

    private sealed class UploadSet : IDisposable
    {
        public List<UploadedFile> Files { get; } = new();
        public List<Stream> Streams { get; } = new();

        public void Dispose()
        {
            foreach (var stream in Streams)
            {
                stream.Dispose();
            }
        }
    }
}
