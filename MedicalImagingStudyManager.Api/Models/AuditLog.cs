using System.ComponentModel.DataAnnotations;

namespace MedicalImagingStudyManager.Api.Models;

public class AuditLog
{
    public int Id { get; set; }

    [Required, MaxLength(80)]
    public string EntityName { get; set; } = string.Empty;

    public int EntityId { get; set; }

    [Required, MaxLength(120)]
    public string Action { get; set; } = string.Empty;

    [Required, MaxLength(500)]
    public string Details { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}
