using System.ComponentModel.DataAnnotations;

namespace MedicalImagingStudyManager.Api.Models;

public class Study
{
    public int Id { get; set; }

    public int PatientId { get; set; }
    public Patient? Patient { get; set; }

    public ImagingModality Modality { get; set; }

    public DateTime StudyDate { get; set; }

    [Required, MaxLength(80)]
    public string BodyPart { get; set; } = string.Empty;

    public StudyPriority Priority { get; set; } = StudyPriority.Routine;

    public StudyStatus Status { get; set; } = StudyStatus.Pending;

    public Report? Report { get; set; }
}
