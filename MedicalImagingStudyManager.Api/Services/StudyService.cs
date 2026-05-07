using MedicalImagingStudyManager.Api.Data;
using MedicalImagingStudyManager.Api.DTOs;
using MedicalImagingStudyManager.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace MedicalImagingStudyManager.Api.Services;

public class StudyService(AppDbContext dbContext, ILogger<StudyService> logger) : IStudyService
{
    public async Task<List<StudyDto>> GetAllAsync(StudyFilterDto filter)
    {
        var query = dbContext.Studies
            .AsNoTracking()
            .Include(study => study.Patient)
            .AsQueryable();

        if (filter.Modality.HasValue)
        {
            query = query.Where(study => study.Modality == filter.Modality.Value);
        }

        if (filter.Status.HasValue)
        {
            query = query.Where(study => study.Status == filter.Status.Value);
        }

        if (filter.Date.HasValue)
        {
            var start = filter.Date.Value.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
            var end = start.AddDays(1);
            query = query.Where(study => study.StudyDate >= start && study.StudyDate < end);
        }

        return await query
            .OrderByDescending(study => study.Priority)
            .ThenBy(study => study.StudyDate)
            .Select(study => ToDto(study))
            .ToListAsync();
    }

    public async Task<StudyDto> CreateAsync(CreateStudyDto request)
    {
        var patientExists = await dbContext.Patients.AnyAsync(patient => patient.Id == request.PatientId);
        if (!patientExists)
        {
            throw new NotFoundException($"Patient with id {request.PatientId} was not found.");
        }

        var study = new Study
        {
            PatientId = request.PatientId,
            Modality = request.Modality,
            StudyDate = request.StudyDate,
            BodyPart = request.BodyPart.Trim(),
            Priority = request.Priority,
            Status = StudyStatus.Pending
        };

        dbContext.Studies.Add(study);
        await dbContext.SaveChangesAsync();

        return await GetByIdAsync(study.Id);
    }

    public async Task<StudyDto> UpdateStatusAsync(int id, StudyStatus status)
    {
        var study = await dbContext.Studies
            .Include(item => item.Patient)
            .FirstOrDefaultAsync(item => item.Id == id);

        if (study is null)
        {
            throw new NotFoundException($"Study with id {id} was not found.");
        }

        if (study.Status != status)
        {
            var oldStatus = study.Status;
            study.Status = status;

            dbContext.AuditLogs.Add(new AuditLog
            {
                EntityName = nameof(Study),
                EntityId = study.Id,
                Action = "StatusChanged",
                Details = $"Study status changed from {oldStatus} to {status}",
                CreatedAt = DateTime.UtcNow
            });

            logger.LogInformation("Study {StudyId} status changed from {OldStatus} to {NewStatus}", study.Id, oldStatus, status);
            await dbContext.SaveChangesAsync();
        }

        return ToDto(study);
    }

    public async Task<StudyDto> MarkUrgentAsync(int id)
    {
        var study = await dbContext.Studies
            .Include(item => item.Patient)
            .FirstOrDefaultAsync(item => item.Id == id);

        if (study is null)
        {
            throw new NotFoundException($"Study with id {id} was not found.");
        }

        study.Priority = StudyPriority.Urgent;
        await dbContext.SaveChangesAsync();

        return ToDto(study);
    }

    private async Task<StudyDto> GetByIdAsync(int id)
    {
        var study = await dbContext.Studies
            .AsNoTracking()
            .Include(item => item.Patient)
            .FirstAsync(item => item.Id == id);

        return ToDto(study);
    }

    private static StudyDto ToDto(Study study)
    {
        return new StudyDto(
            study.Id,
            study.PatientId,
            study.Patient?.FullName ?? "Unknown Patient",
            study.Modality,
            study.StudyDate,
            study.BodyPart,
            study.Priority,
            study.Status);
    }
}
