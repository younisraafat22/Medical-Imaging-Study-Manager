using System.ComponentModel.DataAnnotations;

namespace MedicalImagingStudyManager.Api.Models;

public class Report
{
    public int Id { get; set; }

    public int StudyId { get; set; }
    public Study? Study { get; set; }

    [Required, MaxLength(4000)]
    public string Findings { get; set; } = string.Empty;

    [Required, MaxLength(2000)]
    public string Impression { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
