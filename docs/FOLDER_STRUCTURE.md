# AutoHub Folder Structure & Architecture Documentation

This document provides a comprehensive map of the AutoHub repository layout, detailing the folder structure, architectural responsibilities, and key configuration setups.

---

## Directory & File Tree

Below is the clean structural layout of the AutoHub solution, excluding compiled binaries, temporary logs, local IDE setups, and dependency caches.

```text
AutoHub/
├── .github/
│   └── workflows/
│       └── deploy.yml          # GitHub Actions CI/CD deployment pipeline
├── AutoHub.API/
│   ├── Configurations/         # API-specific configuration bindings (e.g., Hangfire)
│   ├── Controllers/            # Presentation layer REST API controllers
│   ├── Extensions/             # ASP.NET Core service registrations and setup builders
│   ├── HealthChecks/           # Operational readiness and storage dependency checks
│   ├── Middleware/             # Custom request/response handling and exception middleware
│   ├── Properties/
│   │   └── launchSettings.json # Local environment execution profiles
│   ├── Dockerfile              # Multi-stage Docker build recipe for containerization
│   ├── Program.cs              # Application entry point and service bootstrap setup
│   ├── appsettings.json        # Global production and common configurations
│   └── appsettings.Development.json # Development-specific configuration overrides
├── AutoHub.Application/
│   ├── Common/                 # Cross-cutting API response wrapper schemas
│   ├── Configurations/         # Application-level service settings (e.g., JWT)
│   ├── DTOs/                   # Feature-grouped client-server Data Transfer Objects
│   ├── Exceptions/             # Unified business and HTTP exception definitions
│   └── Interfaces/             # Dependency inversion service contracts
├── AutoHub.Domain/
│   ├── Entities/               # Enterprise domain models (core business objects)
│   └── Enums/                  # Immutable domain-specific state enumerations
├── AutoHub.Infrastructure/
│   ├── BackgroundJobs/         # Hangfire job registration and scheduling managers
│   ├── Configuration/          # External infrastructure settings (e.g., AWS S3/MinIO)
│   ├── Migrations/             # EF Core schema migrations for database version control
│   ├── Persistence/            # Database contexts and Fluent API fluent mappings
│   └── Services/               # Concrete implementations of Application layer contracts
├── prometheus/
│   └── prometheus.yml          # Telemetry scrape target configuration for Prometheus
├── AutoHub.slnx                # Modern Visual Studio XML Solution configuration
├── docker-compose.local.yml    # Development-specific multi-container external services setup
├── docker-compose.prod.yml     # Containerized production stack including telemetry services
└── docker-compose.yml          # Core Docker Compose database and storage services
```

---

## Folder & Component Responsibilities

### Root Configuration Files
* **AutoHub.slnx**: A modern, lightweight XML-based solution file defining project inclusions and dependencies.
* **docker-compose.yml**: Orchestrates foundational services (PostgreSQL, Redis, MinIO) for local containerization.
* **docker-compose.local.yml**: Configures standard credentials and overrides for running a consistent local development dependency stack.
* **docker-compose.prod.yml**: Orchestrates the entire containerized production environment, building the API service alongside Redis, Prometheus, and Grafana.

### .github/
* **workflows/deploy.yml**: Continuous Integration & Deployment pipeline that automatically restores, builds, tests, and deploys the release branch to AWS EC2 via container redeployment and conducts automated health checks.

### AutoHub.API (Presentation Layer)
The outer host project that bootstraps the application runtime and exposes external endpoints.
* **Configurations/**: Houses models mapping configuration sections specific to API dashboard security and background tasks.
* **Controllers/**: Maps incoming HTTP requests to corresponding Application services and formats responses.
* **Extensions/**: Organizes service collection bindings, AWS profiles, and rate-limiting middleware to keep the main startup file concise.
* **HealthChecks/**: Verifies the connectivity and operational health of backend data storage systems.
* **Middleware/**: Intercepts HTTP streams to guarantee consistent error responses and trace requests globally.
* **Dockerfile**: Dictates multi-stage build steps, restoring packages in an SDK container and packaging binaries into a lightweight runtime image.

### AutoHub.Application (Core Use Cases Layer)
Implements business workflows and defines application-specific boundaries, rules, and abstraction definitions.
* **Common/**: Holds generic wrappers like `ApiResponse` and `ErrorResponse` to ensure API responses are formatted consistently.
* **Configurations/**: Strongly-typed models mapping configuration blocks relevant to application rules (e.g., JWT security parameters).
* **DTOs/**: Restricts exposed domain properties by exposing lightweight data wrappers tailored for specific input/output views.
* **Exceptions/**: Contains standard, expressive exceptions mapping application failures to predefined HTTP status codes.
* **Interfaces/**: Acts as the dependency inversion gateway, defining service contracts implemented by the Infrastructure layer.

### AutoHub.Domain (Core Domain Layer)
The innermost architecture layer, defining the underlying business entities, state-machines, and domain constants.
* **Entities/**: Holds plain objects (POCOs) describing the core data schema (e.g., User, Vehicle, Dealer, Inquiry, Reservation).
* **Enums/**: Defines stable domain states and types (e.g., inquiry statuses, vehicle sorting, dealer registration status).

### AutoHub.Infrastructure (External Adapters Layer)
Provides operational services and hooks to databases, third-party storage, task schedulers, and other external frameworks.
* **BackgroundJobs/**: Manages the integration, registration, and dispatching of background schedules with Hangfire.
* **Configuration/**: Wraps external service settings including credential mappings for local and cloud object storage.
* **Migrations/**: Standard EF Core database configuration mappings tracking structural database schema updates.
* **Persistence/**: Defines the Entity Framework `DbContext` alongside modular configuration mappers describing column properties and relations.
* **Services/**: Delivers concrete implementations of Application interfaces, including S3/MinIO operations, authentication token generators, and caching wrappers.

### prometheus/
* **prometheus.yml**: Defines global scrape intervals and points Prometheus directly to the containerized ASP.NET API endpoint to gather diagnostics.

---

## Clean Architecture Mapping

The AutoHub solution strictly adheres to the principles of **Clean Architecture**, which separates core business rules from external frameworks, user interfaces, and databases.

```text
┌──────────────────────────────────────────────┐
│                  Presentation                │  (AutoHub.API)
│               ┌──────────────────────────────┤
│               │          Application         │  (AutoHub.Application)
│               │       ┌──────────────────────┤
│               │       │        Domain        │  (AutoHub.Domain) [Innermost]
│               │       └──────────────────────┤
│               │         Dependency Inversion │
│               └──────────────────────────────┤
│                  Infrastructure              │  (AutoHub.Infrastructure)
└──────────────────────────────────────────────┘
```

1. **Domain (Core Rules)**: Located at the absolute center. It has zero external dependencies, referencing no other projects or frameworks. This guarantees that your business logic remains completely unaffected by changes in technology (such as database upgrades or ORM changes).
2. **Application (Business Logic)**: Depends solely on the Domain layer. It acts as the orchestration hub for the system's workflows and uses interfaces to remain decoupled from underlying technical details.
3. **Infrastructure (Data & Adapters)**: Implements the interfaces declared within the Application layer. It communicates with external systems (PostgreSQL, Redis, S3/MinIO). Through dependency inversion, outer-layer implementation details can be refactored or completely swapped without impacting the Core Application or Domain logic.
4. **Presentation (API & Entrypoint)**: Represents the entry point that bootstraps dependencies and parses incoming API requests. It relies on the Application layer to execute work, referencing the Infrastructure layer purely to bootstrap and wire dependencies into the IOC container during startup.

---

## Project Layout Overview

The architectural organization of AutoHub is structured to optimize testability, modularity, and operational visibility across all development environments.

* **Decoupled Architecture**: By utilizing the Dependency Inversion Principle, business rules in the Core Application depend only on interfaces. Concrete implementations (such as database access via EF Core or storage access via S3/MinIO) are injected at runtime by the API container. This prevents business logic from being tightly coupled to specific third-party providers.
* **Consistent Configuration Handling**: Setup parameters are maintained through environment overrides (`appsettings.json` and `appsettings.Development.json`) and injected into specialized option models (under `Configurations` and `Configuration` folders). This structure provides a secure pipeline to inject credentials in production while using defaults locally.
* **Cohesive Infrastructure Stack**:
  * **Databases and Caches**: Docker Compose files configure container dependencies locally, providing a stable local environment mimicking production.
  * **Observability and Monitoring**: The `prometheus/` and `docker-compose.prod.yml` configurations implement telemetry directly out of the box. Metrics exposed by the API are pulled by Prometheus and aggregated in Grafana, giving administrators high visibility into the system's health, latency, and resource metrics.
  * **Automated Delivery (CI/CD)**: The GitHub Actions deployment pipeline coordinates automatic testing, compiling, and deployment. Triggered by release branches, it runs a suite of validation steps before securely rebuilding the production Docker containers on EC2, ensuring that only fully functional code reaches the user.
