using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("categories")]
public class CategoriesController(ICategoryServer eventBookingServer) : ControllerBase
{
    [HttpGet]
    public async Task<IResult> GetCategories()
    {
        return Results.Ok(await eventBookingServer.GetCategories());
    }
}
