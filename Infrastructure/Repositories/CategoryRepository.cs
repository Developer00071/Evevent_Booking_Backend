using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class CategoryRepository(AppDbContext appDbContext) : ICategoryRepository
{
    public IQueryable<Category> GetQuery() => appDbContext.Categories;
    public Task<List<Category>> GetCategories() => appDbContext.Categories.ToListAsync();
    public async Task<Category?> GetCategory(int id) => await appDbContext.Categories.FindAsync(id);
    public Task AddCategory(Category category) => appDbContext.Categories.AddAsync(category).AsTask();
    public void UpdateCategory(Category category) => appDbContext.Categories.Update(category);
    public void DeleteCategory(Category category) => appDbContext.Categories.Remove(category);
    public Task<int> SaveChangesAsync() => appDbContext.SaveChangesAsync();
}
