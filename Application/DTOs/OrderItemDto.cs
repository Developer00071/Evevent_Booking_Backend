using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

public class OrderItemDto
{
    public int TicketTypeId { get; set; }

    [Range(1, 5)]
    public int Quantity { get; set; }
}
