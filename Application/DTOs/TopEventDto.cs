namespace Application.DTOs;

public class TopEventDto
{
    public int EventId { get; set; }
    public string Title { get; set; } = null!;
    public decimal Revenue { get; set; }
}
