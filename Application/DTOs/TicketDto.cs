namespace Application.DTOs;

public class TicketDto
{
    public Guid Code { get; set; }
    public string EventTitle { get; set; } = null!;
    public string TicketTypeName { get; set; } = null!;
    public decimal Price { get; set; }
    public bool IsCheckedIn { get; set; }
}
