using Domain.Enums;

namespace Application.DTOs;

public class EventDetailsDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public EventStatus Status { get; set; }
    public VenueDto Venue { get; set; } = null!;
    public string OrganizerName { get; set; } = null!;
    public List<CategoryDto> Categories { get; set; } = [];
    public List<TicketTypeDto> TicketTypes { get; set; } = [];
    public double AverageRating { get; set; }
    public int ReviewsCount { get; set; }
}
