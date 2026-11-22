using Application.DTOs;

namespace Application.Interfaces;

public interface IEventServer
{
    Task<List<EventDto>> GetEvents(string? city, int? categoryId, DateTime? dateFrom, DateTime? dateTo, decimal? maxPrice, string? search, string sortBy, string sortDir);
    Task<EventDetailsDto> GetEvent(int id);
    Task<EventDto> AddEvent(EventCreateDto dto);
    Task UpdateEvent(int id, EventUpdateDto dto);
    Task PublishEvent(int id);
    Task CancelEvent(int id);
    Task DeleteEvent(int id);
    Task<TicketTypeDto> AddTicketType(int eventId, TicketTypeCreateDto dto);
    Task<List<ReviewDto>> GetReviews(int eventId);
    Task<ReviewDto> AddReview(int eventId, ReviewCreateDto dto);
}
