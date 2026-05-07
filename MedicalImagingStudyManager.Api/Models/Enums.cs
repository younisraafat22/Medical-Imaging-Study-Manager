namespace MedicalImagingStudyManager.Api.Models;

public enum Gender
{
    Unknown = 0,
    Female = 1,
    Male = 2,
    Other = 3
}

public enum ImagingModality
{
    CT = 1,
    MRI = 2,
    XRay = 3,
    Ultrasound = 4,
    Mammography = 5,
    NuclearMedicine = 6
}

public enum StudyPriority
{
    Routine = 1,
    Urgent = 2
}

public enum StudyStatus
{
    Pending = 1,
    InReview = 2,
    Finalized = 3
}

public enum UserRole
{
    Radiologist = 1,
    Technologist = 2,
    ReferringPhysician = 3,
    Admin = 4
}
