using MedicalImagingStudyManager.Api.DTOs;

namespace MedicalImagingStudyManager.Api.Services;

public interface IPatientService
{
    Task<List<PatientDto>> GetAllAsync();
    Task<PatientDto> CreateAsync(CreatePatientDto request);
}
