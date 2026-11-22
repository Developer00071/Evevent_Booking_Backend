using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("orders")]
public class OrdersController(IOrderServer eventBookingServer) : ControllerBase
{
    [HttpPost]
    public async Task<IResult> AddOrder(CreateOrderDto dto)
    {
        var order = await eventBookingServer.AddOrder(dto);
        return Results.Created($"/orders/{order.Id}", order);
    }

    [HttpGet("{id:int}")]
    public async Task<IResult> GetOrder(int id)
    {
        return Results.Ok(await eventBookingServer.GetOrder(id));
    }

    [HttpPost("{id:int}/pay")]
    public async Task<IResult> PayOrder(int id)
    {
        await eventBookingServer.PayOrder(id);
        return Results.NoContent();
    }

    [HttpPost("{id:int}/cancel")]
    public async Task<IResult> CancelOrder(int id)
    {
        await eventBookingServer.CancelOrder(id);
        return Results.NoContent();
    }
}
