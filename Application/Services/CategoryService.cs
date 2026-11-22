using System.Net;
using Application;
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class CategoryService(ICategoryRepository categoryRepository) : ICategoryServer
{
    public async Task<List<CategoryDto>> GetCategories()
    {
        return await categoryRepository.GetQuery().AsNoTracking()
            .OrderBy(category => category.Name)
            .Select(category => new CategoryDto { Id = category.Id, Name = category.Name })
            .ToListAsync();
    }
}
