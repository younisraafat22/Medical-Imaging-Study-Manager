using MedicalImagingStudyManager.Api.Data;
using MedicalImagingStudyManager.Api.DTOs;
using MedicalImagingStudyManager.Api.Models;
using MedicalImagingStudyManager.Api.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace MedicalImagingStudyManager.Tests;

public class PatientServiceTests
{
    [Fact]
    public async Task CreateAsync_AddsPatient()
    {
        using var database = TestDatabase.Create();
        var service = new PatientService(database.Context);

        var result = await service.CreateAsync(new CreatePatientDto
        {
            FullName = "Tobias Martin",
            DateOfBirth = new DateOnly(1988, 6, 15),
            Gender = Gender.Male,
            PatientNumber = "P-20001"
        });

        Assert.True(result.Id > 0);
        Assert.Equal("Tobias Martin", result.FullName);
    }

    [Fact]
    public async Task CreateAsync_RejectsDuplicatePatientNumber()
    {
        using var database = TestDatabase.Create();
        var service = new PatientService(database.Context);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.CreateAsync(new CreatePatientDto
            {
                FullName = "Duplicate Patient",
                DateOfBirth = new DateOnly(1990, 1, 1),
                Gender = Gender.Unknown,
                PatientNumber = "P-10001"
            }));
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
