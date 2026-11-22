using Application.DTOs;
using Domain.Entities;

namespace Application.Services;

public static class EventBookingMapper
{
    public static OrganizerDto MapOrganizer(Organizer organizer)
    {
        return new OrganizerDto
        {
            Id = organizer.Id,
            CompanyName = organizer.CompanyName,
            ContactEmail = organizer.ContactEmail,
            Phone = organizer.Phone
        };
    }

    public static AttendeeDto MapAttendee(Attendee attendee)
    {
        return new AttendeeDto
        {
            Id = attendee.Id,
            FullName = attendee.FullName,
            Email = attendee.Email,
            RegisteredAt = attendee.RegisteredAt
        };
    }

    public static VenueDto MapVenue(Venue venue)
    {
        return new VenueDto
        {
            Id = venue.Id,
            Name = venue.Name,
            Address = venue.Address,
            City = venue.City,
            Capacity = venue.Capacity
        };
    }

    public static TicketTypeDto MapTicketType(TicketType ticketType)
    {
        return new TicketTypeDto
        {
            Id = ticketType.Id,
            Name = ticketType.Name,
            Price = ticketType.Price,
            Quantity = ticketType.Quantity,
            SoldCount = ticketType.SoldCount,
            Available = ticketType.Quantity - ticketType.SoldCount
        };
    }
}
