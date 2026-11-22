using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("events")]
public class EventsController(IEventServer eventBookingServer) : ControllerBase
{
    [HttpGet]
    public async Task<IResult> GetEvents(
        [FromQuery] string? city,
        [FromQuery] int? categoryId,
        [FromQuery] DateTime? dateFrom,
        [FromQuery] DateTime? dateTo,
        [FromQuery] decimal? maxPrice,
        [FromQuery] string? search,
        [FromQuery] string sortBy = "date",
        [FromQuery] string sortDir = "asc")
    {
        return Results.Ok(await eventBookingServer.GetEvents(city, categoryId, dateFrom, dateTo, maxPrice, search, sortBy, sortDir));
    }

    [HttpGet("{id:int}")]
    public async Task<IResult> GetEvent(int id)
    {
        return Results.Ok(await eventBookingServer.GetEvent(id));
    }

    [HttpPost]
    public async Task<IResult> AddEvent(EventCreateDto dto)
    {
        var eventDto = await eventBookingServer.AddEvent(dto);
        return Results.Created($"/events/{eventDto.Id}", eventDto);
    }

    [HttpPut("{id:int}")]
    public async Task<IResult> UpdateEvent(int id, EventUpdateDto dto)
    {
        await eventBookingServer.UpdateEvent(id, dto);
        return Results.NoContent();
    }

    [HttpPost("{id:int}/publish")]
    public async Task<IResult> PublishEvent(int id)
    {
        await eventBookingServer.PublishEvent(id);
        return Results.NoContent();
    }

    [HttpPost("{id:int}/cancel")]
    public async Task<IResult> CancelEvent(int id)
    {
        await eventBookingServer.CancelEvent(id);
        return Results.NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IResult> DeleteEvent(int id)
    {
        await eventBookingServer.DeleteEvent(id);
        return Results.NoContent();
    }

    [HttpPost("{id:int}/ticket-types")]
    public async Task<IResult> AddTicketType(int id, TicketTypeCreateDto dto)
    {
        var ticketType = await eventBookingServer.AddTicketType(id, dto);
        return Results.Json(ticketType, statusCode: StatusCodes.Status201Created);
    }

    [HttpGet("{id:int}/reviews")]
    public async Task<IResult> GetReviews(int id)
    {
        return Results.Ok(await eventBookingServer.GetReviews(id));
    }

    [HttpPost("{id:int}/reviews")]
    public async Task<IResult> AddReview(int id, ReviewCreateDto dto)
    {
        var review = await eventBookingServer.AddReview(id, dto);
        return Results.Json(review, statusCode: StatusCodes.Status201Created);
    }
}
