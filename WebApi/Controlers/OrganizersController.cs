using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("organizers")]
public class OrganizersController(IOrganizerServer eventBookingServer) : ControllerBase
{
    [HttpPost]
    public async Task<IResult> AddOrganizer(OrganizerCreateDto dto)
    {
        var organizer = await eventBookingServer.AddOrganizer(dto);
        return Results.Created("/organizers", organizer);
    }

    [HttpGet]
    public async Task<IResult> GetOrganizers()
    {
        return Results.Ok(await eventBookingServer.GetOrganizers());
    }

    [HttpGet("{id:int}/events")]
    public async Task<IResult> GetOrganizerEvents(int id)
    {
        return Results.Ok(await eventBookingServer.GetOrganizerEvents(id));
    }
}
