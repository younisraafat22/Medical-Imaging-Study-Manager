using MedicalImagingStudyManager.Api.DTOs;
using MedicalImagingStudyManager.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace MedicalImagingStudyManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PatientsController(IPatientService patientService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<PatientDto>>>> GetPatients()
    {
        var patients = await patientService.GetAllAsync();
        return Ok(ApiResponse<List<PatientDto>>.Ok(patients));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<PatientDto>>> CreatePatient(CreatePatientDto request)
    {
        var patient = await patientService.CreateAsync(request);
        return CreatedAtAction(nameof(GetPatients), new { id = patient.Id }, ApiResponse<PatientDto>.Ok(patient, "Patient created"));
    }
}
