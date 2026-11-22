using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

public class AttendeeCreateDto
{
    [Required, MaxLength(100)]
    public string FullName { get; set; } = null!;

    [Required, EmailAddress, MaxLength(150)]
    public string Email { get; set; } = null!;
}
