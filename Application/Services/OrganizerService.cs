using System.Net;
using Application;
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class OrganizerService(IOrganizerRepository organizerRepository, IEventRepository eventRepository) : IOrganizerServer
{
    public async Task<OrganizerDto> AddOrganizer(OrganizerCreateDto dto)
    {
        var organizer = new Organizer
        {
            CompanyName = dto.CompanyName,
            ContactEmail = dto.ContactEmail,
            Phone = dto.Phone
        };
        await organizerRepository.AddOrganizer(organizer);
        await organizerRepository.SaveChangesAsync();
        return EventBookingMapper.MapOrganizer(organizer);
    }

    public async Task<List<OrganizerDto>> GetOrganizers()
    {
        return await organizerRepository.GetQuery().AsNoTracking()
            .OrderBy(organizer => organizer.Id)
            .Select(organizer => new OrganizerDto
            {
                Id = organizer.Id,
                CompanyName = organizer.CompanyName,
                ContactEmail = organizer.ContactEmail,
                Phone = organizer.Phone
            }).ToListAsync();
    }

    public async Task<List<EventDto>> GetOrganizerEvents(int id)
    {
        var exists = await organizerRepository.GetQuery().AnyAsync(organizer => organizer.Id == id);
        if (!exists)
        {
            throw new ApiException((int)HttpStatusCode.NotFound, "Organizer not found.");
        }

        return await eventRepository.GetQuery().AsNoTracking()
            .Where(eventEntity => eventEntity.OrganizerId == id)
            .OrderBy(eventEntity => eventEntity.StartDate)
            .Select(eventEntity => new EventDto
            {
                Id = eventEntity.Id,
                Title = eventEntity.Title,
                StartDate = eventEntity.StartDate,
                City = eventEntity.Venue.City,
                Status = eventEntity.Status,
                MinPrice = eventEntity.TicketTypes.Min(ticketType => (decimal?)ticketType.Price)
            }).ToListAsync();
    }
}
