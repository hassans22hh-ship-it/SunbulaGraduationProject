# ═══════════════════════════════════════════════════════════════
# Stage 1: BUILD — Uses the full SDK to compile the project
# ═══════════════════════════════════════════════════════════════
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

# ── 1. Copy solution and all .csproj files for layer-cached restore ──
COPY SunbulaGraduationProject.slnx .

# BuildingBlocks
COPY BuildingBlocks/SharedKernel/SharedKernel.csproj BuildingBlocks/SharedKernel/

# Host project
COPY Sunbula/Sunbula.csproj Sunbula/

# Integration Tests
COPY Sunbula.IntegrationTests/Sunbula.IntegrationTests.csproj Sunbula.IntegrationTests/

# Modules — Debt
COPY Modules/Debt/Application/DebtApplication.csproj Modules/Debt/Application/
COPY Modules/Debt/DebtInfrastructure/DebtInfrastructure.csproj Modules/Debt/DebtInfrastructure/
COPY Modules/Debt/DebtPresentation/DebtPresentation.csproj Modules/Debt/DebtPresentation/
COPY Modules/Debt/Domain/DebtDomain.csproj Modules/Debt/Domain/

# Modules — Finance
COPY Modules/Finance/Application/FinanceApplication.csproj Modules/Finance/Application/
COPY Modules/Finance/Domain/FinanceDomain.csproj Modules/Finance/Domain/
COPY Modules/Finance/Infrastructure/FinanceInfrastructure.csproj Modules/Finance/Infrastructure/
COPY Modules/Finance/Presentation/FinancePresentation.csproj Modules/Finance/Presentation/

# Modules — StorePlant
COPY Modules/StorePlant/Application/PlantApplication.csproj Modules/StorePlant/Application/
COPY Modules/StorePlant/Domain/PlantDomain.csproj Modules/StorePlant/Domain/
COPY Modules/StorePlant/PlantInfrastructure/PlantInfrastructure.csproj Modules/StorePlant/PlantInfrastructure/
COPY Modules/StorePlant/PlantPresentation/PlantPresentation.csproj Modules/StorePlant/PlantPresentation/

# Modules — TaskManagement
COPY Modules/TaskManagement/Application/TaskApplication.csproj Modules/TaskManagement/Application/
COPY Modules/TaskManagement/Domain/TaskDomain.csproj Modules/TaskManagement/Domain/
COPY Modules/TaskManagement/Presentation/TaskPresentation.csproj Modules/TaskManagement/Presentation/
COPY Modules/TaskManagement/TaskInfrastructure/TaskInfrastructure.csproj Modules/TaskManagement/TaskInfrastructure/

# Modules — TimeTracking
COPY Modules/TimeTracking/Application/TimeTrackingApplication.csproj Modules/TimeTracking/Application/
COPY Modules/TimeTracking/Domain/TimeTrackingDomain.csproj Modules/TimeTracking/Domain/
COPY Modules/TimeTracking/Infrastructure/TimeTrackingInfrastructure.csproj Modules/TimeTracking/Infrastructure/
COPY Modules/TimeTracking/Presentation/TimeTrackingPresentation.csproj Modules/TimeTracking/Presentation/

# Modules — UserIdentity
COPY Modules/UserIdentity/Application/ApplicationIdentity.csproj Modules/UserIdentity/Application/
COPY Modules/UserIdentity/Domain/Domain.csproj Modules/UserIdentity/Domain/
COPY Modules/UserIdentity/Infrastructure/UserIdentityInfrastructure.csproj Modules/UserIdentity/Infrastructure/
COPY Modules/UserIdentity/Presentation/PresentationIdentity.csproj Modules/UserIdentity/Presentation/

# ── 2. Restore NuGet packages (cached until any .csproj changes) ──
RUN dotnet restore SunbulaGraduationProject.slnx

# ── 3. Copy everything else and publish ──
COPY . .

RUN dotnet publish Sunbula/Sunbula.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# ═══════════════════════════════════════════════════════════════
# Stage 2: RUNTIME — Minimal image, no SDK
# ═══════════════════════════════════════════════════════════════
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y --no-install-recommends curl && rm -rf /var/lib/apt/lists/*

# Copy published output from the build stage
COPY --from=build /app/publish .

# ASP.NET 10 defaults to port 8080
EXPOSE 8080

# Set environment
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080

# Health check — Docker will monitor this
HEALTHCHECK --interval=30s --timeout=5s --start-period=10s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "Sunbula.dll"]
