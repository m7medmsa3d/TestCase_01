# 🚀 TestCase Management Microservice (.NET 9)

This repository contains the Core **TestCase Generation & Management Service** for the **Test-Case Generator Graduation Project**.
It handles the complete lifecycle of test cases, from generation logic to multi-level filtering (by Project and Requirement), including custom data-handling rules.

---

## 🛠️ Tech Stack & Architecture

- **Framework:** ASP.NET Core 9.0 (Web API) - *Latest Stable & High-Performance Release*
- **Database ORM:** Entity Framework Core (MySQL)
- **Design Patterns:** Repository Pattern, N-Tier Architecture, Data Transfer Objects (DTO) Isolation
- **Containerization:** Docker & Docker Hub Integration
- **API Documentation:** Swagger / OpenAPI Integration

---

## 📂 Project Structure

The Solution is structured into three decoupled, clean projects ensuring a strict separation of concerns:

```text
Solution 'TestCase_01'
├── 🏢 TestCase_01               # Web API Project (Core)
│   ├── 📂 Controllers           # API Endpoints & Request Orchestration
│   ├── 📄 Program.cs            # Service Configurations, DI, & Middlewares
│   ├── 📄 MappingConfig.cs      # AutoMapper Profiles
│   └── 🐳 Dockerfile            # Containerization & Build Recipe
│
├── 💾 TestCase_01_DataAccess    # Infrastructure Layer
│   ├── 📂 Data                  # DbContext Configurations
│   ├── 📂 Entities              # Core Database Models (Domain)
│   ├── 📂 Migrations            # Database Schema Versioning
│   ├── 📂 Repository            # Data Encapsulation & Query Logic
│   └── 📂 Service               # Core Business Logic Execution
│
└── 📦 TestCase_01_DTO           # Contract Layer
    ├── 📄 TestCaseDTO.cs        # Main Data Representation Template
    ├── 📄 TestCaseRequestDTO.cs # Data Contract for Inbound Requests
    └── 📄 TestCaseResponseDTO.cs# Data Contract for Outbound Responses

## 🔗 The service exposes the following RESTful endpoints for seamless integration with other microservices:

| HTTP Method | Endpoint | Description |
| :--- | :--- | :--- |
| **POST** | `/api/TestCase/create` | Generates & records a new test case scenario. |
| **GET** | `/api/TestCase/{testcaseid}` | Fetches a single test case detail by its unique ID. |
| **GET** | `/api/TestCase/by-project/{projectId}` | Retrieves all test cases associated with a specific Project. |
| **GET** | `/api/TestCase/by-requirement/{requirementId}` | Retrieves all test cases associated with a specific Requirement. |
| **DELETE** | `/api/TestCase/by-requirement/{requirementId}` | Removes all test cases assigned to a specific Requirement. |
| **DELETE** | `/api/TestCase/delete-testcase/{testcaseid}` | Safely deletes a specific testcase via its ID. |


🐳 Docker Deployment & Containerization
The service is fully containerized and hosted publicly on Docker Hub to facilitate rapid CI/CD deployment pipelines.

Docker Hub Repository: mohamedsaadd/testcase-api

Target OS: Linux (Ubuntu-based .NET Runtime)

🏗️ 1. Build Image Locally
To build the Docker image with any new local modifications, execute from the Solution Level (where the .sln resides):

PowerShell
docker build -t mohamedsaadd/testcase-api -f TestCase_01/Dockerfile .

🚀 2. Push Image to Docker Hub
Ensure your authentication is configured via token, then distribute updated images using:

PowerShell
docker push mohamedsaadd/testcase-api:latest
📥 3. Remote Pull Configuration
To deploy or pull the official image into staging or cluster setups:

PowerShell
docker pull mohamedsaadd/testcase-api:latest
