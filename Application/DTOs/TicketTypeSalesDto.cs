namespace Application.DTOs;

public class TicketTypeSalesDto
{
    public int TicketTypeId { get; set; }
    public string TicketTypeName { get; set; } = null!;
    public int TicketsSold { get; set; }
    public decimal Revenue { get; set; }
    public decimal OccupancyPercentage { get; set; }
    public decimal CheckInPercentage { get; set; }
}
