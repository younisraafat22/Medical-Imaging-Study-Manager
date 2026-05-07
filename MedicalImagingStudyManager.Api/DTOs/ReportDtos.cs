using System.ComponentModel.DataAnnotations;

namespace MedicalImagingStudyManager.Api.DTOs;

public record ReportDto(
    int Id,
    int StudyId,
    string Findings,
    string Impression,
    DateTime CreatedAt,
    DateTime UpdatedAt);

public class CreateReportDto
{
    [Range(1, int.MaxValue)]
    public int StudyId { get; set; }

    [Required, MaxLength(4000)]
    public string Findings { get; set; } = string.Empty;

    [Required, MaxLength(2000)]
    public string Impression { get; set; } = string.Empty;
}
