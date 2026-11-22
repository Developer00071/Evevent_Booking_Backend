using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

public class TicketTypeCreateDto
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = null!;

    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue)]
    public int Quantity { get; set; }
}
