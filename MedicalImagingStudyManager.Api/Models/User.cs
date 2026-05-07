using System.ComponentModel.DataAnnotations;

namespace MedicalImagingStudyManager.Api.Models;

public class User
{
    public int Id { get; set; }

    [Required, MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    public UserRole Role { get; set; }

    [Required, EmailAddress, MaxLength(160)]
    public string Email { get; set; } = string.Empty;
}
