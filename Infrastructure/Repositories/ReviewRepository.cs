using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ReviewRepository(AppDbContext appDbContext) : IReviewRepository
{
    public IQueryable<Review> GetQuery() => appDbContext.Reviews;
    public Task<List<Review>> GetReviews() => appDbContext.Reviews.ToListAsync();
    public async Task<Review?> GetReview(int id) => await appDbContext.Reviews.FindAsync(id);
    public Task AddReview(Review review) => appDbContext.Reviews.AddAsync(review).AsTask();
    public void UpdateReview(Review review) => appDbContext.Reviews.Update(review);
    public void DeleteReview(Review review) => appDbContext.Reviews.Remove(review);
    public Task<int> SaveChangesAsync() => appDbContext.SaveChangesAsync();
}
