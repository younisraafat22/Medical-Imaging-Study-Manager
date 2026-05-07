namespace MedicalImagingStudyManager.Api.Services;

public class NotFoundException(string message) : Exception(message);
