using MedicalImagingStudyManager.Api.DTOs;
using MedicalImagingStudyManager.Api.Models;

namespace MedicalImagingStudyManager.Api.Services;

public interface IStudyService
{
    Task<List<StudyDto>> GetAllAsync(StudyFilterDto filter);
    Task<StudyDto> CreateAsync(CreateStudyDto request);
    Task<StudyDto> UpdateStatusAsync(int id, StudyStatus status);
    Task<StudyDto> MarkUrgentAsync(int id);
}
