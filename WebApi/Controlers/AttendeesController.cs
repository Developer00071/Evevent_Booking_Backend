using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("attendees")]
public class AttendeesController(IAttendeeServer eventBookingServer) : ControllerBase
{
    [HttpPost]
    public async Task<IResult> AddAttendee(AttendeeCreateDto dto)
    {
        var attendee = await eventBookingServer.AddAttendee(dto);
        return Results.Created("/attendees", attendee);
    }

    [HttpGet]
    public async Task<IResult> GetAttendees()
    {
        return Results.Ok(await eventBookingServer.GetAttendees());
    }

    [HttpPut("{id:int}/profile")]
    public async Task<IResult> UpdateProfile(int id, AttendeeProfileDto dto)
    {
        await eventBookingServer.UpdateAttendeeProfile(id, dto);
        return Results.NoContent();
    }

    [HttpGet("{id:int}/orders")]
    public async Task<IResult> GetOrders(int id)
    {
        return Results.Ok(await eventBookingServer.GetAttendeeOrders(id));
    }

    [HttpGet("{id:int}/tickets")]
    public async Task<IResult> GetTickets(int id)
    {
        return Results.Ok(await eventBookingServer.GetAttendeeTickets(id));
    }
}
