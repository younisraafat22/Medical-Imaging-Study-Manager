using System.ComponentModel.DataAnnotations;

namespace MedicalImagingStudyManager.Api.Models;

public class Patient
{
    public int Id { get; set; }

    [Required, MaxLength(120)]
    public string FullName { get; set; } = string.Empty;

    public DateOnly DateOfBirth { get; set; }

    public Gender Gender { get; set; }

    [Required, MaxLength(30)]
    public string PatientNumber { get; set; } = string.Empty;

    public List<Study> Studies { get; set; } = [];
}
