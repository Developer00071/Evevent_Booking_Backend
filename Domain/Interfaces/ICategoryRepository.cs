using Domain.Entities;

namespace Domain.Interfaces;

public interface ICategoryRepository
{
    IQueryable<Category> GetQuery();
    Task<List<Category>> GetCategories();
    Task<Category?> GetCategory(int id);
    Task AddCategory(Category category);
    void UpdateCategory(Category category);
    void DeleteCategory(Category category);
    Task<int> SaveChangesAsync();
}
