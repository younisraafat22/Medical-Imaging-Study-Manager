using MedicalImagingStudyManager.Api.DTOs;
using MedicalImagingStudyManager.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace MedicalImagingStudyManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController(IReportService reportService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<ReportDto>>>> GetReports()
    {
        var reports = await reportService.GetAllAsync();
        return Ok(ApiResponse<List<ReportDto>>.Ok(reports));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<ReportDto>>> CreateReport(CreateReportDto request)
    {
        var report = await reportService.CreateAsync(request);
        return CreatedAtAction(nameof(GetReports), new { id = report.Id }, ApiResponse<ReportDto>.Ok(report, "Report created"));
    }
}
