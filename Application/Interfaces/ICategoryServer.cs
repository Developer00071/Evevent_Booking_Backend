using Application.DTOs;

namespace Application.Interfaces;

public interface ICategoryServer
{
    Task<List<CategoryDto>> GetCategories();
}
