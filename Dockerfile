FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY MedicalImagingStudyManager.sln .
COPY MedicalImagingStudyManager.Api/MedicalImagingStudyManager.Api.csproj MedicalImagingStudyManager.Api/
COPY MedicalImagingStudyManager.Tests/MedicalImagingStudyManager.Tests.csproj MedicalImagingStudyManager.Tests/
RUN dotnet restore

COPY . .
RUN dotnet test --configuration Release --no-restore
RUN dotnet publish MedicalImagingStudyManager.Api/MedicalImagingStudyManager.Api.csproj --configuration Release --output /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "MedicalImagingStudyManager.Api.dll"]
