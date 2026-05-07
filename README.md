# Medical Imaging Study Manager

This is a small ASP.NET Core project for managing radiology studies in a hospital workflow.

I built it as an API-first application, then added a simple dashboard UI so the workflow is easier to test and explain. It is not a full hospital system and it does not display real medical images. It manages the information around imaging studies: patients, CT/MRI/X-ray studies, urgency, status, and reports.

## What The App Does

- Create and list patients
- Create and list imaging studies
- Filter studies by modality, date, and status
- Mark studies as urgent
- Change a study status: `Pending`, `InReview`, or `Finalized`
- Create reports for studies
- Add audit log records when study status changes
- Show sample seed data when the app starts
- Provide Swagger documentation for the API
- Provide a small web dashboard for quick demo/testing

## Tech Used

- C#
- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- SQLite
- Swagger
- Serilog
- xUnit
- Docker
- GitHub Actions
- Fly.io deployment
- Simple HTML, CSS, and JavaScript UI

## How To Run

Install the .NET 8 SDK first.

Then run:

```powershell
dotnet restore
dotnet build
dotnet test
dotnet run --project MedicalImagingStudyManager.Api
```

Open the UI:

```text
http://localhost:5000/
```

Open Swagger:

```text
http://localhost:5000/swagger
```

If your terminal shows a different port, use that port instead.

Live Fly.io version:

```text
https://medical-imaging-study-manager-younis.fly.dev/
```

## Main API Endpoints

| Method | Endpoint | What it does |
| --- | --- | --- |
| GET | `/api/patients` | List patients |
| POST | `/api/patients` | Create patient |
| GET | `/api/studies` | List studies |
| GET | `/api/studies?modality=CT&status=Pending` | Filter studies |
| POST | `/api/studies` | Create study |
| PATCH | `/api/studies/{id}/status` | Change study status |
| PATCH | `/api/studies/{id}/urgent` | Mark study urgent |
| GET | `/api/reports` | List reports |
| POST | `/api/reports` | Create report |

## Project Structure

```text
MedicalImagingStudyManager.Api
  Controllers
  Data
  DTOs
  Middleware
  Models
  Services
  wwwroot/ui

MedicalImagingStudyManager.Tests
```

Short explanation:

- `Models`: database entities like Patient, Study, Report, User, and AuditLog
- `DTOs`: request and response objects used by the API
- `Data`: the EF Core database context and seed data
- `Services`: business logic
- `Controllers`: API endpoints
- `Middleware`: global exception handling
- `wwwroot/ui`: the small dashboard UI
- `Tests`: unit tests for the service layer

## Example Study Request

```json
{
  "patientId": 1,
  "modality": "MRI",
  "studyDate": "2026-05-07T10:30:00Z",
  "bodyPart": "Brain",
  "priority": "Routine"
}
```

## Docker

```powershell
docker build -t medical-imaging-study-manager .
docker run --rm -p 8080:8080 medical-imaging-study-manager
```

Then open:

```text
http://localhost:8080/
```

## CI/CD And Fly.io

The project has a GitHub Actions pipeline in:

```text
.github/workflows/ci-cd.yml
```

The pipeline does this:

1. Restores NuGet packages
2. Builds the solution
3. Runs the tests
4. Publishes the API as an artifact
5. Deploys the app to Fly.io when code is pushed to `main`

The Fly.io app config is in:

```text
fly.toml
```

The app name currently used is:

```text
medical-imaging-study-manager-younis
```

To make deployment work, the Fly.io app and volume must exist, and GitHub needs a secret called `FLY_API_TOKEN`.

Basic Fly.io setup:

```powershell
flyctl auth login
flyctl apps create medical-imaging-study-manager-younis
flyctl volumes create medical_imaging_data --app medical-imaging-study-manager-younis --region fra --size 1
flyctl auth token
```

Then add the token in GitHub:

```text
Repo -> Settings -> Secrets and variables -> Actions -> New repository secret
Name: FLY_API_TOKEN
Value: your Fly.io token
```

After that, every push to `main` should deploy to Fly.io.

Current live app:

```text
https://medical-imaging-study-manager-younis.fly.dev/
```

## What I Would Improve Next

- Add login and roles
- Add pagination
- Add database migrations
- Add audit log API endpoints
- Add controller integration tests
- Add DICOM upload support later
- Replace SQLite with SQL Server for a more production-like setup
