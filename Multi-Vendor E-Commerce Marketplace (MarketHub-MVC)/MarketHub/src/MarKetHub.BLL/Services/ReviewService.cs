using MarketHub.BLL.Common;
using MarketHub.BLL.Interfaces;
using MarketHub.DAL.Data;
using MarketHub.DAL.Entities;
using MarKetHub.DAL.Enums;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace MarketHub.BLL.Services;

public class ReviewService : IReviewService
{
    private readonly ApplicationDbContext _context;

    public ReviewService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> CanReviewAsync(
        string customerId,
        int productId,
        CancellationToken ct = default)
    {
        // Check whether the customer has a delivered order item for this product.
        var hasDeliveredOrder = await _context.OrderItems
            .AsNoTracking()
            .AnyAsync(oi =>
                oi.ProductId == productId &&
                oi.Order.CustomerId == customerId &&
                oi.FulfillmentStatus == FulfillmentStatus.Delivered,
                ct);

        if (!hasDeliveredOrder)
            return false;

        // Check whether the customer has already reviewed this product.
        var alreadyReviewed = await _context.Reviews
            .AsNoTracking()
            .AnyAsync(r =>
                r.CustomerId == customerId &&
                r.ProductId == productId,
                ct);

        return !alreadyReviewed;
    }

    public async Task<Result> AddAsync(
        string customerId,
        int productId,
        int rating,
        string? comment,
        CancellationToken ct = default)
    {
        // Validate rating.
        if (rating < 1 || rating > 5)
            return Result.Fail("Rating must be between 1 and 5.");

        // Validate comment length.
        if (comment?.Length > 1000)
            return Result.Fail("Comment cannot exceed 1000 characters.");

        // Ensure the product exists first.
        var productExists = await _context.Products
            .AsNoTracking()
            .AnyAsync(p => p.Id == productId, ct);

        if (!productExists)
            return Result.NotFoundResult("Product not found.");

        // Check review eligibility.
        var canReview = await CanReviewAsync(customerId, productId, ct);

        if (!canReview)
            return Result.Fail(
                "You can review this product only after it has been delivered, and only once.");

        var review = new Review
        {
            ProductId = productId,
            CustomerId = customerId,
            Rating = rating,
            Comment = string.IsNullOrWhiteSpace(comment)
                ? null
                : comment.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        _context.Reviews.Add(review);

        try
        {
            await _context.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex)
            when (ex.InnerException is SqlException sqlException
                  && (sqlException.Number == 2601 || sqlException.Number == 2627))
        {
            // Handle duplicate review caused by a concurrent request.
            return Result.Fail(
                "You have already reviewed this product.");
        }

        return Result.Ok("Review added successfully.");
    }
}