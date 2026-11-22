using System.Net;
using Application;
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class EventService(IEventRepository eventRepository, IVenueRepository venueRepository, IOrganizerRepository organizerRepository, ICategoryRepository categoryRepository, ITicketTypeRepository ticketTypeRepository, IReviewRepository reviewRepository, ITicketRepository ticketRepository, IAttendeeRepository attendeeRepository, IOrderRepository orderRepository) : IEventServer
{
    public async Task<List<EventDto>> GetEvents(
        string? city, int? categoryId, DateTime? dateFrom, DateTime? dateTo, decimal? maxPrice,
        string? search, string sortBy, string sortDir)
    {
        if (dateFrom.HasValue && dateTo.HasValue && dateFrom > dateTo)
        {
            throw new ApiException((int)HttpStatusCode.BadRequest, "dateFrom must be earlier than dateTo.");
        }

        var query = eventRepository.GetQuery().AsNoTracking()
            .Where(eventEntity => eventEntity.Status == EventStatus.Published);
        if (!string.IsNullOrWhiteSpace(city))
        {
            query = query.Where(eventEntity => eventEntity.Venue.City == city);
        }
        if (categoryId.HasValue)
        {
            query = query.Where(eventEntity => eventEntity.Categories.Any(category => category.Id == categoryId.Value));
        }
        if (dateFrom.HasValue)
        {
            query = query.Where(eventEntity => eventEntity.StartDate >= dateFrom.Value);
        }
        if (dateTo.HasValue)
        {
            query = query.Where(eventEntity => eventEntity.StartDate <= dateTo.Value);
        }
        if (maxPrice.HasValue)
        {
            query = query.Where(eventEntity => eventEntity.TicketTypes.Any()
                && eventEntity.TicketTypes.Min(ticketType => ticketType.Price) <= maxPrice.Value);
        }
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(eventEntity => eventEntity.Title.Contains(search));
        }

        var descending = string.Equals(sortDir, "desc", StringComparison.OrdinalIgnoreCase);
        query = sortBy.ToLowerInvariant() switch
        {
            "price" => descending
                ? query.OrderByDescending(eventEntity => eventEntity.TicketTypes.Min(ticketType => (decimal?)ticketType.Price))
                : query.OrderBy(eventEntity => eventEntity.TicketTypes.Min(ticketType => (decimal?)ticketType.Price)),
            "title" => descending
                ? query.OrderByDescending(eventEntity => eventEntity.Title)
                : query.OrderBy(eventEntity => eventEntity.Title),
            "date" => descending
                ? query.OrderByDescending(eventEntity => eventEntity.StartDate)
                : query.OrderBy(eventEntity => eventEntity.StartDate),
            _ => throw new ApiException((int)HttpStatusCode.BadRequest, "sortBy must be date, price, or title.")
        };
        if (!string.Equals(sortDir, "asc", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(sortDir, "desc", StringComparison.OrdinalIgnoreCase))
        {
            throw new ApiException((int)HttpStatusCode.BadRequest, "sortDir must be asc or desc.");
        }

        return await query.Select(eventEntity => new EventDto
            {
                Id = eventEntity.Id,
                Title = eventEntity.Title,
                StartDate = eventEntity.StartDate,
                City = eventEntity.Venue.City,
                Status = eventEntity.Status,
                MinPrice = eventEntity.TicketTypes.Min(ticketType => (decimal?)ticketType.Price)
            }).ToListAsync();
    }

    public async Task<EventDetailsDto> GetEvent(int id)
    {
        var eventEntity = await eventRepository.GetQuery().AsNoTracking()
            .Include(item => item.Venue)
            .Include(item => item.Organizer)
            .Include(item => item.Categories)
            .Include(item => item.TicketTypes)
            .Include(item => item.Reviews).ThenInclude(review => review.Attendee)
            .FirstOrDefaultAsync(item => item.Id == id)
            ?? throw new ApiException((int)HttpStatusCode.NotFound, "Event not found.");
        return new EventDetailsDto
        {
            Id = eventEntity.Id,
            Title = eventEntity.Title,
            Description = eventEntity.Description,
            StartDate = eventEntity.StartDate,
            EndDate = eventEntity.EndDate,
            Status = eventEntity.Status,
            Venue = EventBookingMapper.MapVenue(eventEntity.Venue),
            OrganizerName = eventEntity.Organizer.CompanyName,
            Categories = eventEntity.Categories.Select(category => new CategoryDto
            {
                Id = category.Id,
                Name = category.Name
            }).ToList(),
            TicketTypes = eventEntity.TicketTypes.Select(EventBookingMapper.MapTicketType).ToList(),
            AverageRating = eventEntity.Reviews.Count == 0 ? 0 : eventEntity.Reviews.Average(review => review.Rating),
            ReviewsCount = eventEntity.Reviews.Count
        };
    }

    public async Task<EventDto> AddEvent(EventCreateDto dto)
    {
        ServiceValidation.ValidateDates(dto.StartDate, dto.EndDate);
        var venue = await venueRepository.GetQuery().AnyAsync(item => item.Id == dto.VenueId);
        var organizer = await organizerRepository.GetQuery().AnyAsync(item => item.Id == dto.OrganizerId);
        var categories = await categoryRepository.GetQuery()
            .Where(category => dto.CategoryIds.Contains(category.Id)).ToListAsync();
        if (!venue || !organizer || categories.Count != dto.CategoryIds.Distinct().Count())
        {
            throw new ApiException((int)HttpStatusCode.BadRequest, "Venue, organizer, or category does not exist.");
        }

        var eventEntity = new Event
        {
            Title = dto.Title,
            Description = dto.Description,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            VenueId = dto.VenueId,
            OrganizerId = dto.OrganizerId,
            Status = EventStatus.Draft,
            Categories = categories
        };
        await eventRepository.AddEvent(eventEntity);
        await eventRepository.SaveChangesAsync();
        return new EventDto
        {
            Id = eventEntity.Id,
            Title = eventEntity.Title,
            StartDate = eventEntity.StartDate,
            Status = eventEntity.Status,
            City = await venueRepository.GetQuery().Where(item => item.Id == eventEntity.VenueId)
                .Select(item => item.City).FirstAsync()
        };
    }

    public async Task UpdateEvent(int id, EventUpdateDto dto)
    {
        ServiceValidation.ValidateDates(dto.StartDate, dto.EndDate);
        var eventEntity = await eventRepository.GetQuery().Include(item => item.Categories)
            .FirstOrDefaultAsync(item => item.Id == id)
            ?? throw new ApiException((int)HttpStatusCode.NotFound, "Event not found.");
        if (eventEntity.Status != EventStatus.Draft)
        {
            throw new ApiException((int)HttpStatusCode.Conflict, "Only draft events can be edited.");
        }

        var venueExists = await venueRepository.GetQuery().AnyAsync(item => item.Id == dto.VenueId);
        var organizerExists = await organizerRepository.GetQuery().AnyAsync(item => item.Id == dto.OrganizerId);
        var categories = await categoryRepository.GetQuery().Where(category => dto.CategoryIds.Contains(category.Id)).ToListAsync();
        if (!venueExists || !organizerExists || categories.Count != dto.CategoryIds.Distinct().Count())
        {
            throw new ApiException((int)HttpStatusCode.BadRequest, "Venue, organizer, or category does not exist.");
        }

        eventEntity.Title = dto.Title;
        eventEntity.Description = dto.Description;
        eventEntity.StartDate = dto.StartDate;
        eventEntity.EndDate = dto.EndDate;
        eventEntity.VenueId = dto.VenueId;
        eventEntity.OrganizerId = dto.OrganizerId;
        eventEntity.Categories = categories;
        await eventRepository.SaveChangesAsync();
    }

    public async Task PublishEvent(int id)
    {
        var eventEntity = await eventRepository.GetQuery().Include(item => item.TicketTypes)
            .FirstOrDefaultAsync(item => item.Id == id)
            ?? throw new ApiException((int)HttpStatusCode.NotFound, "Event not found.");
        if (eventEntity.Status != EventStatus.Draft || eventEntity.TicketTypes.Count == 0)
        {
            throw new ApiException((int)HttpStatusCode.Conflict, "Only a draft event with ticket types can be published.");
        }

        eventEntity.Status = EventStatus.Published;
        await eventRepository.SaveChangesAsync();
    }

    public async Task CancelEvent(int id)
    {
        await orderRepository.InTransactionAsync(async () =>
        {
            var eventEntity = await eventRepository.GetQuery().FirstOrDefaultAsync(item => item.Id == id)
                ?? throw new ApiException((int)HttpStatusCode.NotFound, "Event not found.");
            var orders = await orderRepository.GetQuery()
                .Where(order => order.Tickets.Any(ticket => ticket.TicketType.EventId == id)
                    && (order.Status == OrderStatus.Paid || order.Status == OrderStatus.Pending))
                .Include(order => order.Tickets).ThenInclude(ticket => ticket.TicketType)
                .ToListAsync();
            if (orders.Any(order => order.Tickets.Any(ticket => ticket.TicketType.EventId != id)))
            {
                throw new ApiException(
                    (int)HttpStatusCode.Conflict,
                    "Event cannot be cancelled while an order contains tickets for multiple events.");
            }

            foreach (var order in orders)
            {
                order.Status = OrderStatus.Cancelled;
                foreach (var ticket in order.Tickets.Where(ticket => ticket.TicketType.EventId == id))
                {
                    ticket.TicketType.SoldCount--;
                }
            }

            eventEntity.Status = EventStatus.Cancelled;
            await eventRepository.SaveChangesAsync();
        });
    }

    public async Task DeleteEvent(int id)
    {
        var eventEntity = await eventRepository.GetQuery().FirstOrDefaultAsync(item => item.Id == id)
            ?? throw new ApiException((int)HttpStatusCode.NotFound, "Event not found.");
        eventEntity.IsDeleted = true;
        await eventRepository.SaveChangesAsync();
    }

    public async Task<TicketTypeDto> AddTicketType(int eventId, TicketTypeCreateDto dto)
    {
        var eventExists = await eventRepository.GetQuery().AnyAsync(item => item.Id == eventId);
        if (!eventExists)
        {
            throw new ApiException((int)HttpStatusCode.NotFound, "Event not found.");
        }

        var ticketType = new TicketType
        {
            EventId = eventId,
            Name = dto.Name,
            Price = dto.Price,
            Quantity = dto.Quantity
        };
        await ticketTypeRepository.AddTicketType(ticketType);
        await eventRepository.SaveChangesAsync();
        return EventBookingMapper.MapTicketType(ticketType);
    }

    public async Task<List<ReviewDto>> GetReviews(int eventId)
    {
        var eventExists = await eventRepository.GetQuery().AnyAsync(item => item.Id == eventId);
        if (!eventExists)
        {
            throw new ApiException((int)HttpStatusCode.NotFound, "Event not found.");
        }

        return await reviewRepository.GetQuery().AsNoTracking().Where(review => review.EventId == eventId)
            .OrderByDescending(review => review.CreatedAt)
            .Select(review => new ReviewDto
            {
                Id = review.Id,
                AttendeeId = review.AttendeeId,
                AttendeeName = review.Attendee.FullName,
                Rating = review.Rating,
                Comment = review.Comment,
                CreatedAt = review.CreatedAt
            }).ToListAsync();
    }

    public async Task<ReviewDto> AddReview(int eventId, ReviewCreateDto dto)
    {
        var eventEntity = await eventRepository.GetQuery().FirstOrDefaultAsync(item => item.Id == eventId)
            ?? throw new ApiException((int)HttpStatusCode.NotFound, "Event not found.");
        if (eventEntity.EndDate >= DateTime.UtcNow)
        {
            throw new ApiException((int)HttpStatusCode.Conflict, "Reviews are available after the event ends.");
        }

        var checkedIn = await ticketRepository.GetQuery().AnyAsync(ticket =>
            ticket.AttendeeId == dto.AttendeeId &&
            ticket.TicketType.EventId == eventId &&
            ticket.IsCheckedIn &&
            ticket.Order.Status == OrderStatus.Paid);
        if (!checkedIn)
        {
            throw new ApiException((int)HttpStatusCode.Conflict, "A checked-in ticket is required to review this event.");
        }
        if (await reviewRepository.GetQuery().AnyAsync(review => review.EventId == eventId && review.AttendeeId == dto.AttendeeId))
        {
            throw new ApiException((int)HttpStatusCode.Conflict, "A review already exists for this attendee.");
        }

        var attendee = await attendeeRepository.GetQuery().FirstOrDefaultAsync(item => item.Id == dto.AttendeeId)
            ?? throw new ApiException((int)HttpStatusCode.NotFound, "Attendee not found.");
        var reviewEntity = new Review
        {
            EventId = eventId,
            AttendeeId = dto.AttendeeId,
            Rating = dto.Rating,
            Comment = dto.Comment,
            CreatedAt = DateTime.UtcNow
        };
        await reviewRepository.AddReview(reviewEntity);
        await eventRepository.SaveChangesAsync();
        return new ReviewDto
        {
            Id = reviewEntity.Id,
            AttendeeId = attendee.Id,
            AttendeeName = attendee.FullName,
            Rating = reviewEntity.Rating,
            Comment = reviewEntity.Comment,
            CreatedAt = reviewEntity.CreatedAt
        };
    }
}
