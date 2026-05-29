# 🚀 TestCase Management Microservice (.NET 9)

This repository contains the Core **TestCase Generation & Management Service** for the **Test-Case Generator Graduation Project**.
It handles the complete lifecycle of test cases, from generation logic to multi-level filtering (by Project and Requirement), including custom data-handling rules.

---

## 🛠️ Tech Stack & Architecture

- **Framework:** ASP.NET Core 9.0 (Web API) - *Latest Stable & High-Performance Release*
- **Database ORM:** Entity Framework Core (MySQL)
- **Design Patterns:** Repository Pattern, N-Tier Architecture, Data Transfer Objects (DTO) Isolation
- **Containerization:** Docker & Docker Hub Integration
- **API Documentation:** Swagger / OpenAPI Integration / Postman

---

## ✨ Key Features & Business Logic

The `TestCase` microservice delivers core functionalities engineered for robustness, clean data flow, and high performance:

### 🧠 1. Dynamic TestCase Ingestion & Storage (`/create`)
- **Automated Inter-Service Pipeline:** Systematically consumes pre-generated test case metadata forwarded directly from the primary Java-based processing engine.
- **Inbound Validation:** Strictly enforces payload structures via decoupled `TestCaseRequestDTO` data contracts, validating incoming JSON streams before persisting them safely into the MySQL database.
  
### 🔍 2. Multi-Level Relational Filtering (`by-project` & `by-requirement`)
- **Granular Queries:** Implements optimized relational mapping allowing client applications or other microservices to query test cases down to specific Projects or explicit Sub-Requirements.
- **Optimized Data Flow:** Prevents over-fetching by utilizing thin `TestCaseDTO` mappings, ensuring faster JSON serialization and minimal bandwidth consumption over HTTP.

### 🛡️ 3. Safe Cascade Deletion (`by-requirement`)
- **Bulk Cleanup Operations:** Provides an endpoint to cleanly purge or isolate entire suites of test cases tied to a specific requirement once that requirement is deprecated or changed.
- **Data Integrity:** Coordinated through the **Repository Pattern** to maintain consistent database state across connected entity dependencies.

### 📦 4. Isolated Data Architecture (N-Tier Isolation)
- **Zero-Leaking Schemas:** Database entities inside `TestCase_01_DataAccess` never leak directly to the client. All communications are strictly gated behind the `TestCase_01_DTO` layer.
- **Enterprise Ready:** Prepared for seamless async operations and microservices  integration  if scaled in future graduation project pipelines.


---

## 🔄 Inter-Service Communication & Data Flow

To ensure high availability and decoupled processing, this service acts as a specialized persistence and lifecycle broker within a distributed orchestration pipeline:

1. **Frontend Request Initiation:** The Frontend client triggers the primary generation workflow by directly calling the Java-based microservice.
2. **Backend Service-to-Service Dispatch:** Upon successfully processing the generation logic, the Java service securely acts as a client, dispatching the complete test case metadata downstream to this `.NET 9` service via an HTTP POST request (`/api/TestCase/create`).
3. **Context-Bounded Persistence:** This service validates and persists the inbound payload, mapping the test suites directly to their respective `projectId` and `requirementId` inside the MySQL database.
4. **Decoupled Autonomy:** Once the initial ingestion flow is completed, the Frontend bypasses the Java engine and communicates directly with this `.NET 9` service to execute all subsequent lifecycle actions (such as granular filtering, specific fetching, and safe purging) via independent endpoints.

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
