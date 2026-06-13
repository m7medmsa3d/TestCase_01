# 🚀 TestCase Management Service

## 📌 Description

**TestCase Management Service** is a dedicated **ASP.NET Core 9 microservice** built to manage the full lifecycle of generated software test cases inside a distributed test-case generation platform.

The service receives generated test cases from an external generation engine, stores them in a MySQL database, organizes them by user, project, and requirement, and provides clean API endpoints for retrieval, dashboard summaries, soft deletion, and exporting test cases in multiple formats.

It is designed to work as an independent backend service that can be deployed with Docker and integrated with Java/Spring microservices, frontend clients, gateways, and other system components.

## 📚 Table of Contents

- [🧭 Overview](#overview)
- [🛠️ Tech Stack](#tech-stack)
- [🏗️ Architecture](#architecture)
- [📂 Project Structure](#project-structure)
- [✨ Main Features](#main-features)
- [🗄️ Database Schema](#database-schema)
- [⚙️ Configuration](#configuration)
- [🔌 API Endpoints](#api-endpoints)
- [💻 Run Locally](#run-locally)
- [🧱 Database Migrations](#database-migrations)
- [🐳 Docker](#docker)
- [🧩 Docker Compose](#docker-compose)
- [🔗 Integration With Java/Spring Services](#integration-with-javaspring-services)

## 🧭 Overview

The TestCase Management Service is responsible for receiving generated test cases, saving them in MySQL, and exposing APIs that allow other services or frontend applications to retrieve, delete, summarize, and export test case data.

The service works with three main identifiers:

- `userId`
- `projectId`
- `requirementId`

Each test case belongs to a project, requirement, and user. Each test case can also contain multiple test case steps.

## 🛠️ Tech Stack

- ASP.NET Core 9 Web API
- Entity Framework Core
- MySQL
- MySql.EntityFrameworkCore
- AutoMapper
- Repository Pattern
- Service Layer
- DTO Layer
- Swagger / OpenAPI
- Docker
- Docker Compose
- Health Checks

## 🏗️ Architecture

The solution is separated into three projects:

- `TestCase_01`: ASP.NET Core Web API project.
- `TestCase_01_DataAccess`: database entities, DbContext, repositories, migrations, and services.
- `TestCase_01_DTO`: request and response DTOs used by the API.

This separation keeps API controllers, business logic, database access, and external request/response contracts in different layers.

## 📂 Project Structure

```text
TestCase_01/
  Controllers/
    TestCaseController.cs
  Properties/
    launchSettings.json
  appsettings.json
  appsettings.Development.json
  Dockerfile
  docker-compose.yml
  MappingConfig.cs
  Program.cs
  TestCase_01.csproj
  TestCase_01.http
  TestCase_01.sln

TestCase_01_DataAccess/
  Data/
    ApplicationDbContext.cs
  Entities/
    APIResponse.cs
    TestCase.cs
    TestCaseStep.cs
  Migrations/
  Repository/
    IRepository/
    Repository.cs
    TestCaseRepository.cs
    UnitOfWork.cs
  Service/
    IService/
    TestCaseService.cs
  TestCase_01_DataAccess.csproj

TestCase_01_DTO/
  DashboardBreakdownResponse.cs
  DashboardTrendPointResponse.cs
  ProfileActivityResponse.cs
  ProfileStatsResponse.cs
  TestCaseDTO.cs
  TestCaseRequestDTO.cs
  TestCaseResponseDTO.cs
  TestcaseTypeBreakdownResponse.cs
  TestCase_01_DTO.csproj
```

## ✨ Main Features

- Create and persist generated test cases.
- Store multiple steps for each test case.
- Retrieve a single test case by testcase ID and user ID.
- Retrieve all test cases for a project.
- Retrieve all test cases for a requirement.
- Soft delete test cases by testcase ID.
- Soft delete all test cases under a requirement.
- Return dashboard/profile summary data for a user.
- Export test cases by testcase, requirement, or project.
- Provide a `/health` endpoint for container health checks.
- Run as a Docker container on port `2000`.

## 🗄️ Database Schema

The service uses Entity Framework Core with MySQL.

Main tables:

- `TESTCASE`
- `test_case_steps`

The `TestCase` entity is mapped to:

```text
TESTCASE
```

The `TestCaseStep` entity is mapped to:

```text
test_case_steps
```

Important: table names can be case-sensitive on Linux MySQL containers. `TESTCASE`, `TestCase`, and `testcase` may be treated as different table names.

## ⚙️ Configuration

The service reads the database connection string from `ConnectionStrings:DefaultConnection`.

Example:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=testcasedb;Port=3306;Database=testcasedb;User=root;Password=root;"
  }
}
```

When running in Docker Compose, it is recommended to pass the connection string through environment variables:

```yaml
environment:
  ASPNETCORE_URLS: http://+:2000
  ConnectionStrings__DefaultConnection: Server=testcasedb;Port=3306;Database=testcasedb;User=root;Password=root;
```

Important: inside Docker Compose, containers communicate using the service name and the internal container port. For MySQL, this is usually `3306`, not the host-mapped port.

## 🔌 API Endpoints

The controller uses absolute routes, so the routes are mounted directly from the root path.

### 🏥 Health

```http
GET /health
```

### 📥 Create Test Cases

```http
POST /create
```

Creates a batch of test cases.

Example request body:

```json
{
  "requirementId": 1,
  "projectId": 1,
  "userId": 5,
  "testcases": [
    {
      "type": "Functional",
      "title": "User can login with valid credentials",
      "steps": [
        "Open login page",
        "Enter valid email and password",
        "Click login"
      ],
      "expectedResult": "User is redirected to the dashboard"
    }
  ]
}
```

### 🔍 Query Endpoints

```http
GET /testcase/{testcaseid}/{userId}
GET /projects/{projectId}/{userId}
GET /requirements/{requirementId}/{userId}
GET /internal/users/{userId}/summary
```

### 🗑️ Delete Endpoints

```http
DELETE /testcase/{testcaseid}/{userId}
DELETE /requirements/{requirementId}/{userId}
```

The delete behavior is soft delete.

### 📤 Export Endpoints

```http
GET /{testcaseId}/{userId}/{format}/export
GET /requirements/{requirementId}/{userId}/{format}/export
GET /projects/{projectId}/{userId}/{format}/export
```

Supported format values:

- `excel`
- `xlsx`
- `word`
- `docx`
- `pdf`

## 💻 Run Locally

Prerequisites:

- .NET 9 SDK
- MySQL
- EF Core CLI tools

Install EF Core tools if needed:

```bash
dotnet tool install --global dotnet-ef
```

Restore dependencies:

```bash
dotnet restore
```

Run the API:

```bash
dotnet run --project TestCase_01/TestCase_01.csproj
```

The service listens on:

```text
http://localhost:2000
```

Test the health endpoint:

```bash
curl http://localhost:2000/health
```

## 🧱 Database Migrations

Apply existing migrations:

```bash
dotnet ef database update --project TestCase_01_DataAccess --startup-project TestCase_01
```

Create a new migration:

```bash
dotnet ef migrations add InitialCreate --project TestCase_01_DataAccess --startup-project TestCase_01
```

Apply the migration:

```bash
dotnet ef database update --project TestCase_01_DataAccess --startup-project TestCase_01
```

If the service is deployed in Docker, make sure the database schema is created before using the API endpoints, or enable automatic migrations during startup.

Example startup migration code after `var app = builder.Build();`:

```csharp
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}
```

This requires migrations to already exist in the project.

## 🐳 Docker

Build the image from the repository root:

```bash
docker build -t mohamedsaadd/testcase01-service:latest -f TestCase_01/Dockerfile .
```

Run the image:

```bash
docker run --rm -p 2000:2000 mohamedsaadd/testcase01-service:latest
```

Run with a connection string:

```bash
docker run --rm -p 2000:2000 \
  -e ASPNETCORE_URLS=http://+:2000 \
  -e ConnectionStrings__DefaultConnection="Server=host.docker.internal;Port=3306;Database=testcasedb;User=root;Password=root;" \
  mohamedsaadd/testcase01-service:latest
```

Push to Docker Hub:

```bash
docker login
docker push mohamedsaadd/testcase01-service:latest
```

Pull from Docker Hub:

```bash
docker pull mohamedsaadd/testcase01-service:latest
```

## 🔗 Integration With Java/Spring Services

If a Java/Spring service runs in the same Docker Compose network, it should call this .NET API using the Docker Compose service name:

```text
http://testcase-api:2000
```

Do not use `localhost` between containers. Inside a container, `localhost` refers to the same container, not another service.

## 📝 Notes

- The API should listen on port `2000`.
- The MySQL database must exist before applying migrations.
- The database schema must contain `TESTCASE` and `test_case_steps`.
- The service is designed to be called by other services in a microservices environment.
