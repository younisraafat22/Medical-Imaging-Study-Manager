using MedicalImagingStudyManager.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace MedicalImagingStudyManager.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Study> Studies => Set<Study>();
    public DbSet<Report> Reports => Set<Report>();
    public DbSet<User> Users => Set<User>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Patient>()
            .HasIndex(patient => patient.PatientNumber)
            .IsUnique();

        modelBuilder.Entity<Study>()
            .HasOne(study => study.Patient)
            .WithMany(patient => patient.Studies)
            .HasForeignKey(study => study.PatientId);

        modelBuilder.Entity<Report>()
            .HasOne(report => report.Study)
            .WithOne(study => study.Report)
            .HasForeignKey<Report>(report => report.StudyId);

        modelBuilder.Entity<User>()
            .HasIndex(user => user.Email)
            .IsUnique();

        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Patient>().HasData(
            new Patient { Id = 1, FullName = "Mariam Schneider", DateOfBirth = new DateOnly(1978, 4, 12), Gender = Gender.Female, PatientNumber = "P-10001" },
            new Patient { Id = 2, FullName = "Jonas Weber", DateOfBirth = new DateOnly(1965, 9, 3), Gender = Gender.Male, PatientNumber = "P-10002" },
            new Patient { Id = 3, FullName = "Elena Rossi", DateOfBirth = new DateOnly(1991, 1, 27), Gender = Gender.Female, PatientNumber = "P-10003" });

        modelBuilder.Entity<Study>().HasData(
            new Study { Id = 1, PatientId = 1, Modality = ImagingModality.MRI, StudyDate = new DateTime(2026, 5, 1, 9, 30, 0, DateTimeKind.Utc), BodyPart = "Brain", Priority = StudyPriority.Routine, Status = StudyStatus.InReview },
            new Study { Id = 2, PatientId = 2, Modality = ImagingModality.CT, StudyDate = new DateTime(2026, 5, 2, 14, 15, 0, DateTimeKind.Utc), BodyPart = "Chest", Priority = StudyPriority.Urgent, Status = StudyStatus.Pending },
            new Study { Id = 3, PatientId = 3, Modality = ImagingModality.XRay, StudyDate = new DateTime(2026, 5, 3, 11, 0, 0, DateTimeKind.Utc), BodyPart = "Left Knee", Priority = StudyPriority.Routine, Status = StudyStatus.Finalized });

        modelBuilder.Entity<Report>().HasData(
            new Report { Id = 1, StudyId = 3, Findings = "No acute fracture. Mild medial compartment narrowing.", Impression = "Mild degenerative change without acute osseous abnormality.", CreatedAt = new DateTime(2026, 5, 3, 12, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2026, 5, 3, 12, 0, 0, DateTimeKind.Utc) });

        modelBuilder.Entity<User>().HasData(
            new User { Id = 1, Name = "Dr. Sarah Klein", Role = UserRole.Radiologist, Email = "sarah.klein@hospital.example" },
            new User { Id = 2, Name = "Noah Fischer", Role = UserRole.Technologist, Email = "noah.fischer@hospital.example" });

        modelBuilder.Entity<AuditLog>().HasData(
            new AuditLog { Id = 1, EntityName = "Study", EntityId = 3, Action = "StatusChanged", Details = "Study status changed from InReview to Finalized", CreatedAt = new DateTime(2026, 5, 3, 12, 5, 0, DateTimeKind.Utc) });
    }
}
