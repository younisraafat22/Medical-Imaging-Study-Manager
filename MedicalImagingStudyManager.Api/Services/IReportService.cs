using MedicalImagingStudyManager.Api.DTOs;

namespace MedicalImagingStudyManager.Api.Services;

public interface IReportService
{
    Task<List<ReportDto>> GetAllAsync();
    Task<ReportDto> CreateAsync(CreateReportDto request);
}
