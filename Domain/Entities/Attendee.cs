using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Attendee
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string FullName { get; set; } = null!;

    [Required]
    [MaxLength(150)]
    [EmailAddress]
    public string Email { get; set; } = null!;

    public DateTime RegisteredAt { get; set; }
    public AttendeeProfile? Profile { get; set; }
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}
