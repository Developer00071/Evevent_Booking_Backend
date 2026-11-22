using Domain.Entities;

namespace Domain.Interfaces;

public interface IReviewRepository
{
    IQueryable<Review> GetQuery();
    Task<List<Review>> GetReviews();
    Task<Review?> GetReview(int id);
    Task AddReview(Review review);
    void UpdateReview(Review review);
    void DeleteReview(Review review);
    Task<int> SaveChangesAsync();
}
