# 🚀 TestCase Management Microservice (.NET 9)

This repository contains the Core **TestCase Generation & Management Service** for the **Test-Case Generator Graduation Project**.
It handles the complete lifecycle of test cases, from generation logic to multi-level filtering (by Project and Requirement), including custom data-handling rules.

---

## 🛠️ Tech Stack & Architecture

- **Framework:** ASP.NET Core 9.0 (Web API) - *Latest Stable & High-Performance Release*
- **Database ORM:** Entity Framework Core (MySQL)
- **Design Patterns:** Repository Pattern, N-Tier Architecture, Data Transfer Objects (DTO) Isolation
- **Containerization:** Docker with Built-in Health Checks (`curl`) & Docker Hub Integration
- **API Documentation:** Swagger / OpenAPI Integration / Postman

---

## ✨ Key Features & Business Logic

The `TestCase` microservice delivers core functionalities engineered for robustness, clean data flow, and high performance:

### 🧠 1. Dynamic TestCase Ingestion (`/create`)
- **Automated Inter-Service Pipeline:** Systematically consumes pre-generated test case metadata forwarded directly from the primary Java-based processing engine.
- **Inbound Validation:** Strictly enforces payload structures via decoupled `TestCaseRequestDTO` data contracts.
- **Optimized for Bulk Actions:** Returns a clean `204 NoContent` status upon success, ensuring seamless compatibility and error-free parsing with the upstream Java service.
  
### 🔍 2. Multi-Level Relational Filtering & Exporting
- **Granular Queries:** Implements optimized relational mapping allowing client applications to query test cases down to specific Projects, Requirements, or complete User Summaries.
- **Multi-Format Export Engine:** Supports exporting targeted test case suites to multiple formats directly via dedicated download endpoints (e.g., `/{testcaseId}/{userId}/{format}/export`).

### 🛡️ 3. Safe Cascade Deletion
- **Bulk Cleanup Operations:** Provides explicit endpoints to cleanly purge entire suites of test cases tied to a specific requirement or an individual testcase model (`DELETE` verbs).
- **Data Integrity:** Coordinated through the **Repository Pattern** to maintain consistent database state across connected entity dependencies.

### 📦 4. Isolated Data Architecture (N-Tier Isolation)
- **Zero-Leaking Schemas:** Database entities inside `TestCase_01_DataAccess` never leak directly to the client. All communications are strictly gated behind the `TestCase_01_DTO` layer.

---

## 🔄 Inter-Service Communication & Data Flow

To ensure high availability and decoupled processing, this service acts as a specialized persistence and lifecycle broker within a distributed orchestration pipeline:

1. **Frontend Request Initiation:** The Frontend client triggers the primary generation workflow by directly calling the Java-based microservice.
2. **Backend Service-to-Service Dispatch:** Upon successfully processing the generation logic, the Java service securely acts as a client, dispatching the complete test case metadata downstream to this `.NET 9` service via an HTTP POST request (`/api/TestCase/create`).
3. **Context-Bounded Persistence:** This service validates and persists the inbound payload, mapping the test suites directly to their respective `projectId` and `requirementId` inside the MySQL database.
4. **Decoupled Autonomy:** Once the initial ingestion flow is completed, the Frontend bypasses the Java engine and communicates directly with this `.NET 9` service to execute all subsequent lifecycle actions (such as granular filtering, specific fetching, exporting, and safe purging) via independent endpoints.

---

## 🛑 API Endpoints (Swagger Specifications)

### 📥 Ingestion & Creation
- `POST /api/TestCase/create` - Bulk Ingestion of Test Cases *(Returns 204 No Content)*

### 🔍 Querying & Management
- `GET /api/TestCase/testcase/{testcaseid}/{userId}` - Fetch specific Test Case details
- `GET /api/TestCase/projects/{projectId}/{userId}` - Fetch all Test Cases linked to a Project
- `GET /api/TestCase/requirements/{requirementId}/{userId}` - Fetch all Test Cases linked to a Requirement
- `GET /api/TestCase/internal/users/{userId}/summary` - Fetch a structural overview/dashboard stats for a user

### 💾 Export Engine
- `GET /api/TestCase/{testcaseId}/{userId}/{format}/export` - Export a single Test Case
- `GET /api/TestCase/requirements/{requirementId}/{userId}/{format}/export` - Export full Requirement test suite
- `GET /api/TestCase/projects/{projectId}/{userId}/{format}/export` - Export full Project test suite

### 🗑️ Deletion / Purging
- `DELETE /api/TestCase/testcase/{testcaseid}/{userId}` - Delete a specific Test Case
- `DELETE /api/TestCase/requirements/{requirementId}/{userId}` - Delete an entire suite under a Requirement

---

## 📂 Project Structure

The Solution is structured into three decoupled, clean projects ensuring a strict separation of concerns:

```text
Solution 'TestCase_01'
├── 🏢 TestCase_01               # Web API Project (Core)
│   ├── 📂 Controllers           # API Endpoints & Request Orchestration
│   ├── 📄 Program.cs            # Service Configurations, Health Checks, DI, & Middlewares
│   ├── 📄 MappingConfig.cs      # AutoMapper Profiles
│   └── 🐳 Dockerfile            # Containerization with automated curl health probe
│
├── 💾 TestCase_01_DataAccess    # Infrastructure Layer
│   ├── 📂 Data                  # DbContext Configurations & MySQL Target
│   ├── 📂 Entities              # Core Database Models (Domain Entities)
│   ├── 📂 Migrations            # Database Schema Versioning
│   ├── 📂 Repository            # Data Encapsulation & Query Logic
│   └── 📂 Service               # Core Business Logic Execution
│
└── 📦 TestCase_01_DTO           # Contract Layer (Data Schemas)
    ├── 📄 TestCaseDTO.cs                  # Main Data Representation Template
    ├── 📄 TestCaseRequestDTO.cs           # Data Contract for Inbound Requests
    ├── 📄 DashboardBreakdownResponse.cs   # Metrics Schema for System Breakdown
    ├── 📄 DashboardTrendPointResponse.cs  # Metrics Schema for System Analytics
    ├── 📄 ProfileActivityResponse.cs      # User Engagement Logs Schema
    ├── 📄 ProfileStatsResponse.cs         # Aggregate User Activity Metrics
    └── 📄 TestcaseTypeBreakdownResponse.cs# Categorized TestCase Metrics Schema
