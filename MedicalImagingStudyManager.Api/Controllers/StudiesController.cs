using MedicalImagingStudyManager.Api.DTOs;
using MedicalImagingStudyManager.Api.Models;
using MedicalImagingStudyManager.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace MedicalImagingStudyManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudiesController(IStudyService studyService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<StudyDto>>>> GetStudies([FromQuery] StudyFilterDto filter)
    {
        var studies = await studyService.GetAllAsync(filter);
        return Ok(ApiResponse<List<StudyDto>>.Ok(studies));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<StudyDto>>> CreateStudy(CreateStudyDto request)
    {
        var study = await studyService.CreateAsync(request);
        return CreatedAtAction(nameof(GetStudies), new { id = study.Id }, ApiResponse<StudyDto>.Ok(study, "Study created"));
    }

    [HttpPatch("{id:int}/status")]
    public async Task<ActionResult<ApiResponse<StudyDto>>> UpdateStatus(int id, UpdateStudyStatusDto request)
    {
        var study = await studyService.UpdateStatusAsync(id, request.Status);
        return Ok(ApiResponse<StudyDto>.Ok(study, "Study status updated"));
    }

    [HttpPatch("{id:int}/urgent")]
    public async Task<ActionResult<ApiResponse<StudyDto>>> MarkUrgent(int id)
    {
        var study = await studyService.MarkUrgentAsync(id);
        return Ok(ApiResponse<StudyDto>.Ok(study, "Study marked urgent"));
    }
}
