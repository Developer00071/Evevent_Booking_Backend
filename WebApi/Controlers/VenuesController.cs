using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("venues")]
public class VenuesController(IVenueServer eventBookingServer) : ControllerBase
{
    [HttpPost]
    public async Task<IResult> AddVenue(VenueCreateDto dto)
    {
        var venue = await eventBookingServer.AddVenue(dto);
        return Results.Created("/venues", venue);
    }

    [HttpGet]
    public async Task<IResult> GetVenues()
    {
        return Results.Ok(await eventBookingServer.GetVenues());
    }

    [HttpDelete("{id:int}")]
    public async Task<IResult> DeleteVenue(int id)
    {
        await eventBookingServer.DeleteVenue(id);
        return Results.NoContent();
    }
}
