using MedicalImagingStudyManager.Api.Data;
using MedicalImagingStudyManager.Api.DTOs;
using MedicalImagingStudyManager.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace MedicalImagingStudyManager.Api.Services;

public class PatientService(AppDbContext dbContext) : IPatientService
{
    public async Task<List<PatientDto>> GetAllAsync()
    {
        return await dbContext.Patients
            .AsNoTracking()
            .OrderBy(patient => patient.FullName)
            .Select(patient => new PatientDto(
                patient.Id,
                patient.FullName,
                patient.DateOfBirth,
                patient.Gender,
                patient.PatientNumber))
            .ToListAsync();
    }

    public async Task<PatientDto> CreateAsync(CreatePatientDto request)
    {
        var patientNumberExists = await dbContext.Patients
            .AnyAsync(patient => patient.PatientNumber == request.PatientNumber);

        if (patientNumberExists)
        {
            throw new InvalidOperationException($"Patient number '{request.PatientNumber}' already exists.");
        }

        var patient = new Patient
        {
            FullName = request.FullName.Trim(),
            DateOfBirth = request.DateOfBirth,
            Gender = request.Gender,
            PatientNumber = request.PatientNumber.Trim()
        };

        dbContext.Patients.Add(patient);
        await dbContext.SaveChangesAsync();

        return new PatientDto(patient.Id, patient.FullName, patient.DateOfBirth, patient.Gender, patient.PatientNumber);
    }
}
