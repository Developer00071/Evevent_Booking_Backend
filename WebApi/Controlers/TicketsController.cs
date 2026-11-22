using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("tickets")]
public class TicketsController(ITicketServer eventBookingServer) : ControllerBase
{
    [HttpGet("{code:guid}")]
    public async Task<IResult> GetTicket(Guid code)
    {
        return Results.Ok(await eventBookingServer.GetTicket(code));
    }

    [HttpPost("{code:guid}/check-in")]
    public async Task<IResult> CheckIn(Guid code)
    {
        await eventBookingServer.CheckIn(code);
        return Results.NoContent();
    }
}
