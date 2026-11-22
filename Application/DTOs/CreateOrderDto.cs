namespace Application.DTOs;

public class CreateOrderDto
{
    public int AttendeeId { get; set; }
    public List<OrderItemDto> Items { get; set; } = [];
}
