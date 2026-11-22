using System.Net;
using Application;
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class VenueService(IVenueRepository venueRepository, IEventRepository eventRepository) : IVenueServer
{
    public async Task<VenueDto> AddVenue(VenueCreateDto dto)
    {
        var venue = new Venue
        {
            Name = dto.Name,
            Address = dto.Address,
            City = dto.City,
            Capacity = dto.Capacity
        };
        await venueRepository.AddVenue(venue);
        await venueRepository.SaveChangesAsync();
        return new VenueDto
        {
            Id = venue.Id,
            Name = venue.Name,
            Address = venue.Address,
            City = venue.City,
            Capacity = venue.Capacity
        };
    }

    public async Task<List<VenueDto>> GetVenues()
    {
        return await venueRepository.GetQuery().AsNoTracking()
            .OrderBy(venue => venue.Id)
            .Select(venue => new VenueDto
            {
                Id = venue.Id,
                Name = venue.Name,
                Address = venue.Address,
                City = venue.City,
                Capacity = venue.Capacity
            }).ToListAsync();
    }

    public async Task DeleteVenue(int id)
    {
        var venue = await venueRepository.GetVenue(id)
            ?? throw new ApiException((int)HttpStatusCode.NotFound, "Venue not found.");
        var hasEvents = await eventRepository.GetQuery().IgnoreQueryFilters().AnyAsync(eventEntity => eventEntity.VenueId == id);
        if (hasEvents)
        {
            throw new ApiException((int)HttpStatusCode.Conflict, "Venue has events and cannot be deleted.");
        }

        venueRepository.DeleteVenue(venue);
        await venueRepository.SaveChangesAsync();
    }
}
