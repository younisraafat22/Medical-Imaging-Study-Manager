using System.ComponentModel.DataAnnotations;
using MedicalImagingStudyManager.Api.Models;

namespace MedicalImagingStudyManager.Api.DTOs;

public record StudyDto(
    int Id,
    int PatientId,
    string PatientName,
    ImagingModality Modality,
    DateTime StudyDate,
    string BodyPart,
    StudyPriority Priority,
    StudyStatus Status);

public class CreateStudyDto
{
    [Range(1, int.MaxValue)]
    public int PatientId { get; set; }

    public ImagingModality Modality { get; set; }

    public DateTime StudyDate { get; set; }

    [Required, MaxLength(80)]
    public string BodyPart { get; set; } = string.Empty;

    public StudyPriority Priority { get; set; } = StudyPriority.Routine;
}

public class UpdateStudyStatusDto
{
    public StudyStatus Status { get; set; }
}

public class StudyFilterDto
{
    public ImagingModality? Modality { get; set; }
    public DateOnly? Date { get; set; }
    public StudyStatus? Status { get; set; }
}
