using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("reports")]
public class ReportsController(IReportServer eventBookingServer) : ControllerBase
{
    [HttpGet("events/{id:int}/sales")]
    public async Task<IResult> GetEventSales(int id)
    {
        return Results.Ok(await eventBookingServer.GetEventSales(id));
    }

    [HttpGet("top-events")]
    public async Task<IResult> GetTopEvents([FromQuery] int count = 5)
    {
        return Results.Ok(await eventBookingServer.GetTopEvents(count));
    }
}
