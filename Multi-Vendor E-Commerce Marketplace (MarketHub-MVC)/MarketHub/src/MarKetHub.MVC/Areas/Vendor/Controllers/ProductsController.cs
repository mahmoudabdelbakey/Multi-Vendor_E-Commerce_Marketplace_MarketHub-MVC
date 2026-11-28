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
/// Added later: images (TASK-34), concurrent edit message (TASK-35), search / paging / show-hide / quick stock (TASK-37).
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

    /// <summary>TASK-37. Search by name (q) + paging. The search text travels in the URL, so paging and redirects keep it.</summary>
    [HttpGet]
    public async Task<IActionResult> Index(string? q = null, int page = 1, CancellationToken ct = default)
    {
        var search = NormalizeSearch(q);
        var result = await _products.ListAsync(VendorId, search, page, PageSize, ct);

        // Deleting the last product of the last page leaves an empty page: go to the last page that exists.
        if (result.Items.Count == 0 && page > 1)
        {
            return RedirectToAction(nameof(Index), new { q = search, page = Math.Max(1, result.TotalPages) });
        }

        return View(new ProductListViewModel { Products = result, Search = search });
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

        // TASK-35: the version the form was opened with is required. Without it the service could not detect a concurrent edit.
        if (string.IsNullOrWhiteSpace(model.RowVersion))
        {
            ModelState.AddModelError(string.Empty, "This form is out of date. Please reload the page and try again.");
        }

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
            // TASK-35: if the saved product has a newer version than the form, somebody else changed it: show the conflict screen.
            var latest = await _products.GetForEditAsync(VendorId, id, ct);
            if (latest is null)
            {
                return NotFound();
            }

            if (!string.Equals(latest.Input.RowVersion, model.RowVersion, StringComparison.Ordinal))
            {
                return await ShowConflictAsync(model, latest, ct);
            }

            ModelState.AddModelError(string.Empty, result.Message);
            return await ReShowEditAsync(model, ct);
        }

        Flash(result);
        return RedirectToAction(nameof(Index));
    }

    // ------------------------------------------------------------------ images (TASK-34)
    // Uploading new images is part of the Create / Edit forms. These two actions change the images that already exist.
    // They never touch the Product row, so they do not clash with a form that is open in another tab (RowVersion).

    [HttpPost]
    public async Task<IActionResult> SetPrimaryImage([FromRoute] int id, int imageId, CancellationToken ct)
    {
        var result = await _products.SetPrimaryImageAsync(VendorId, id, imageId, ct);
        return RedirectWithResult(result, nameof(Edit), new { id });
    }

    /// <summary>The service deletes the database row first and the file from the storage only after that succeeded.</summary>
    [HttpPost]
    public async Task<IActionResult> DeleteImage([FromRoute] int id, int imageId, CancellationToken ct)
    {
        var result = await _products.DeleteImageAsync(VendorId, id, imageId, ct);
        return RedirectWithResult(result, nameof(Edit), new { id });
    }

    // ------------------------------------------------------------------ delete

    /// <summary>
    /// POST only. A product with orders is hidden instead of deleted: the service decides and its message tells the vendor.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Delete([FromRoute] int id, string? q = null, int page = 1, CancellationToken ct = default)
    {
        var result = await _products.DeleteAsync(VendorId, id, ct);
        return RedirectWithResult(result, nameof(Index), new { q = NormalizeSearch(q), page });
    }

    // ------------------------------------------------------------------ quick actions from the list (TASK-37)
    // Both change the Product row, so a form that is open in another tab gets the "changed by someone else" message (TASK-35).

    /// <summary>Show / hide in the store. The service checks the owner; isActive is the NEW value the button asks for.</summary>
    [HttpPost]
    public async Task<IActionResult> ToggleActive([FromRoute] int id, bool isActive, string? q = null, int page = 1, CancellationToken ct = default)
    {
        var result = await _products.SetActiveAsync(VendorId, id, isActive, ct);
        return RedirectWithResult(result, nameof(Index), new { q = NormalizeSearch(q), page });
    }

    /// <summary>Quick stock change. The range (0 - 1,000,000) is checked by the service.</summary>
    [HttpPost]
    public async Task<IActionResult> UpdateStock([FromRoute] int id, int? quantity, string? q = null, int page = 1, CancellationToken ct = default)
    {
        if (quantity is null)
        {
            Flash(false, "Please enter a whole number for the stock quantity.");
            return RedirectToAction(nameof(Index), new { q = NormalizeSearch(q), page });
        }

        var result = await _products.AdjustStockAsync(VendorId, id, quantity.Value, ct);
        return RedirectWithResult(result, nameof(Index), new { q = NormalizeSearch(q), page });
    }

    // ------------------------------------------------------------------ helpers

    /// <summary>Trims the search text, turns blank into null and caps it (a product name is at most 150 characters).</summary>
    private static string? NormalizeSearch(string? q)
    {
        var text = q?.Trim();
        if (string.IsNullOrEmpty(text))
        {
            return null;
        }

        return text.Length > 100 ? text[..100] : text;
    }

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
    /// TASK-35. Somebody else saved this product after the form was opened. Nothing is overwritten.
    /// The form keeps what the vendor typed, but carries the NEW RowVersion: pressing Save again is a conscious decision
    /// to overwrite. The values that are saved right now are shown above the form so the vendor can compare.
    /// </summary>
    private async Task<IActionResult> ShowConflictAsync(ProductFormViewModel model, ProductEditDto latest, CancellationToken ct)
    {
        ModelState.Remove(nameof(ProductFormViewModel.RowVersion)); // otherwise the hidden field would re-post the old value
        model.RowVersion = latest.Input.RowVersion;
        model.LatestSaved = latest;
        model.ExistingImages = latest.Images;
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
