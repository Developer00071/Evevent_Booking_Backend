using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Organizer
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string CompanyName { get; set; } = null!;

    [Required]
    [MaxLength(150)]
    [EmailAddress]
    public string ContactEmail { get; set; } = null!;

    [MaxLength(30)]
    public string? Phone { get; set; }

    public ICollection<Event> Events { get; set; } = new List<Event>();
}
