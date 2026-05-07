using MedicalImagingStudyManager.Api.Data;
using MedicalImagingStudyManager.Api.DTOs;
using MedicalImagingStudyManager.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace MedicalImagingStudyManager.Api.Services;

public class ReportService(AppDbContext dbContext) : IReportService
{
    public async Task<List<ReportDto>> GetAllAsync()
    {
        return await dbContext.Reports
            .AsNoTracking()
            .OrderByDescending(report => report.CreatedAt)
            .Select(report => ToDto(report))
            .ToListAsync();
    }

    public async Task<ReportDto> CreateAsync(CreateReportDto request)
    {
        var study = await dbContext.Studies.FirstOrDefaultAsync(item => item.Id == request.StudyId);
        if (study is null)
        {
            throw new NotFoundException($"Study with id {request.StudyId} was not found.");
        }

        var reportExists = await dbContext.Reports.AnyAsync(report => report.StudyId == request.StudyId);
        if (reportExists)
        {
            throw new InvalidOperationException($"Study {request.StudyId} already has a report.");
        }

        var now = DateTime.UtcNow;
        var report = new Report
        {
            StudyId = request.StudyId,
            Findings = request.Findings.Trim(),
            Impression = request.Impression.Trim(),
            CreatedAt = now,
            UpdatedAt = now
        };

        var oldStatus = study.Status;
        study.Status = StudyStatus.Finalized;

        dbContext.Reports.Add(report);

        if (oldStatus != StudyStatus.Finalized)
        {
            dbContext.AuditLogs.Add(new AuditLog
            {
                EntityName = nameof(Study),
                EntityId = study.Id,
                Action = "StatusChanged",
                Details = $"Study status changed from {oldStatus} to {StudyStatus.Finalized}",
                CreatedAt = now
            });
        }

        dbContext.AuditLogs.Add(new AuditLog
        {
            EntityName = nameof(Study),
            EntityId = study.Id,
            Action = "ReportCreated",
            Details = "Report was created and study was finalized",
            CreatedAt = now
        });

        await dbContext.SaveChangesAsync();

        return ToDto(report);
    }

    private static ReportDto ToDto(Report report)
    {
        return new ReportDto(report.Id, report.StudyId, report.Findings, report.Impression, report.CreatedAt, report.UpdatedAt);
    }
}
