using MedicalImagingStudyManager.Api.Data;
using MedicalImagingStudyManager.Api.DTOs;
using MedicalImagingStudyManager.Api.Models;
using MedicalImagingStudyManager.Api.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace MedicalImagingStudyManager.Tests;

public class StudyServiceTests
{
    [Fact]
    public async Task GetAllAsync_FiltersByModalityAndStatus()
    {
        using var database = TestDatabase.Create();
        var service = new StudyService(database.Context, NullLogger<StudyService>.Instance);

        var studies = await service.GetAllAsync(new StudyFilterDto
        {
            Modality = ImagingModality.CT,
            Status = StudyStatus.Pending
        });

        Assert.Single(studies);
        Assert.Equal(ImagingModality.CT, studies[0].Modality);
        Assert.Equal(StudyStatus.Pending, studies[0].Status);
    }

    [Fact]
    public async Task UpdateStatusAsync_AddsAuditLogWhenStatusChanges()
    {
        using var database = TestDatabase.Create();
        var service = new StudyService(database.Context, NullLogger<StudyService>.Instance);
        var auditCountBefore = await database.Context.AuditLogs.CountAsync();

        var study = await service.UpdateStatusAsync(2, StudyStatus.InReview);
        var auditCountAfter = await database.Context.AuditLogs.CountAsync();

        Assert.Equal(StudyStatus.InReview, study.Status);
        Assert.Equal(auditCountBefore + 1, auditCountAfter);
    }

    [Fact]
    public async Task MarkUrgentAsync_UpdatesPriority()
    {
        using var database = TestDatabase.Create();
        var service = new StudyService(database.Context, NullLogger<StudyService>.Instance);

        var study = await service.MarkUrgentAsync(1);

        Assert.Equal(StudyPriority.Urgent, study.Priority);
    }
}

file sealed class TestDatabase : IDisposable
{
    private readonly SqliteConnection _connection;

    private TestDatabase(SqliteConnection connection, AppDbContext context)
    {
        _connection = connection;
        Context = context;
    }

    public AppDbContext Context { get; }

    public static TestDatabase Create()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        var context = new AppDbContext(options);
        context.Database.EnsureCreated();

        return new TestDatabase(connection, context);
    }

    public void Dispose()
    {
        Context.Dispose();
        _connection.Dispose();
    }
}
