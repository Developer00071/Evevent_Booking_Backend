using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

public class OrganizerCreateDto
{
    [Required, MaxLength(100)]
    public string CompanyName { get; set; } = null!;

    [Required, EmailAddress, MaxLength(150)]
    public string ContactEmail { get; set; } = null!;

    [MaxLength(30)]
    public string? Phone { get; set; }
}
