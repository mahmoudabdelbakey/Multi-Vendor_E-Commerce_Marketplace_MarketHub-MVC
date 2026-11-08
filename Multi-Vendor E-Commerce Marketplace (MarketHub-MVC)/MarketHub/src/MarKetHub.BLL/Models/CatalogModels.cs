namespace MarketHub.BLL.Models;

public class ProductQuery
{
    public const int MaxPageSize = 48;
    public static readonly string[] SortOptions = { "newest", "price_asc", "price_desc", "name" };

    public string? Search { get; set; }
    public int? CategoryId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? Sort { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 12;

    /// <summary>Clamps untrusted query-string values into safe ranges / whitelists.</summary>
    public ProductQuery Normalize()
    {
        Page = Page < 1 ? 1 : Page;
        PageSize = PageSize < 1 ? 12 : Math.Min(PageSize, MaxPageSize);
        Sort = Sort is not null && SortOptions.Contains(Sort) ? Sort : "newest";
        Search = string.IsNullOrWhiteSpace(Search) ? null : Search.Trim();
        if (Search is { Length: > 100 })
        {
            Search = Search[..100];
        }

        return this;
    }
}

public record ProductListItemDto(
    int Id, string Name, decimal Price, int StockQuantity,
    string CategoryName, string StoreName, string? ImagePath);

public record ProductImageDto(int Id, string Path, bool IsPrimary);

public record ReviewDto(int Id, string CustomerName, int Rating, string? Comment, DateTime CreatedAt);

public record ProductDetailsDto(
    int Id, string Name, string? Description, decimal Price, int StockQuantity,
    int CategoryId, string CategoryName, int VendorId, string StoreName,
    IReadOnlyList<ProductImageDto> Images, double AverageRating, int ReviewCount,
    IReadOnlyList<ReviewDto> Reviews);

public record CategoryDto(int Id, string Name, string Slug, bool IsActive, int ProductCount);
