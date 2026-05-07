using System.ComponentModel.DataAnnotations;
using MedicalImagingStudyManager.Api.Models;

namespace MedicalImagingStudyManager.Api.DTOs;

public record PatientDto(
    int Id,
    string FullName,
    DateOnly DateOfBirth,
    Gender Gender,
    string PatientNumber);

public class CreatePatientDto
{
    [Required, MaxLength(120)]
    public string FullName { get; set; } = string.Empty;

    public DateOnly DateOfBirth { get; set; }

    public Gender Gender { get; set; }

    [Required, MaxLength(30)]
    public string PatientNumber { get; set; } = string.Empty;
}
