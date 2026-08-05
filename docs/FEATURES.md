# AutoHub API - Features

This document provides a comprehensive list of features implemented in the AutoHub ASP.NET Core solution.

## Authentication & Authorization

- **JWT Authentication**: JSON Web Token (JWT) Bearer authentication for secure, stateless API request authorization.
- **Role-Based Access Control (RBAC)**: Fine-grained authorization enforcing permissions across `Buyer`, `Dealer`, and `Admin` user roles.
- **Password Hashing**: Secure password hashing and verification using `BCrypt.Net-Next`.
- **Dealer Onboarding**: Dealer profile creation and administrative approval/rejection workflow.

## Vehicle Management

- **Vehicle Lifecycle CRUD**: Full management of vehicle listings with status states (`Draft`, `Published`, `Reserved`, `Sold`, `Archived`).
- **Dealer-Scoped Management**: Inventory control restricted so dealers can only modify their own vehicle listings.
- **Vehicle Image Handling**: Support for multi-image uploads, sequence ordering, primary image designation, and deletion.
- **Reservation Workflow**: Vehicle reservation capabilities for buyers with automated expiration handling.
- **Inquiry System**: Communication system allowing buyers to submit inquiries on vehicle listings and dealers to send responses.
- **Favorites & Wishlists**: Capability for buyers to add, view, and remove vehicles from their personal favorites list.
- **Administrative Controls**: Admin functionality to unpublish listings, override vehicle statuses, and review pending submissions.
- **Analytics & Dashboard**: Administrative dashboard aggregating system metrics, top vehicles, top dealers, and active reservations.

## Search & Filtering

- **Multi-Criteria Search**: Dynamic vehicle filtering by make, model, year range, price range, mileage range, fuel type, transmission, body type, status, and dealer.
- **Flexible Sorting**: Sorting capabilities by price, manufacture year, mileage, creation date, and calculated trending score.
- **Paginated Responses**: Standardized pagination (`PaginatedResponse<T>`) across vehicle search, inquiries, reservations, and favorites.
- **Filter Metadata**: Dedicated endpoint returning available makes, models, body types, fuel types, and transmission options for UI filters.

## Media & Storage

- **Dual Storage Providers**: Flexible storage abstraction supporting both AWS S3 cloud storage and MinIO object storage.
- **Upload Validation**: File upload verification enforcing allowed extensions (`.jpg`, `.jpeg`, `.png`, `.webp`) and maximum file size constraints.
- **Pre-Signed URLs**: Generation of secure, time-limited pre-signed URLs for public media access.
- **Storage Health Checking**: Integrated health check monitor verifying bucket accessibility and storage provider connectivity.

## Background Jobs

- **Hangfire Job Scheduler**: Background processing engine powered by Hangfire with PostgreSQL persistence (`Hangfire.PostgreSql`).
- **Reservation Expiration Job**: Scheduled recurring job (`expire-reservations`) that automatically releases expired vehicle reservations back to published status.
- **Trending Score Calculation**: Scheduled recurring job (`recalculate-trending`) calculating vehicle popularity scores based on user interactions.
- **Analytics Sync Job**: Scheduled recurring job (`flush-analytics`) syncing atomic Redis interaction counters to PostgreSQL database entities.
- **Secured Hangfire Dashboard**: Password-protected Hangfire UI dashboard (`/hangfire`) configured with Basic Authentication (`Hangfire.Dashboard.BasicAuthorization`).

## Security

- **Rate Limiting**: IP-based fixed-window rate limiting (`System.Threading.RateLimiting`) covering login, registration, search, and global endpoints.
- **CORS Policies**: Cross-Origin Resource Sharing policy configured with environment-specific origins for development and production environments.
- **Global Exception Middleware**: Centralized error-handling middleware (`ExceptionMiddleware`) delivering formatted JSON error responses.
- **Payload & Input Validation**: Strict validation of incoming request models and file uploads to prevent unauthorized operations.

## Caching

- **Redis Distributed Cache**: Integration with Redis (`StackExchange.Redis` & `Microsoft.Extensions.Caching.StackExchangeRedis`) for distributed caching.
- **High-Speed Counter Operations**: Real-time atomic increment and decrement of vehicle view, favorite, inquiry, and reservation metrics in Redis.
- **Cache Service Layer**: Abstraction wrapper (`ICacheService`) handling object serialization, retrieval, time-based expiration, and deletion.
- **Trending Data Caching**: Cached calculation of top trending vehicles to optimize query performance and lower database read load.

## Monitoring & Observability

- **Prometheus Metrics**: HTTP request telemetry and custom metrics collection using `prometheus-net.AspNetCore` exposed at `/metrics`.
- **Prometheus Server**: Containerized Prometheus instance configured (`prometheus.yml`) for periodic scraping of API metrics.
- **Grafana Visualization**: Containerized Grafana service integrated for metrics monitoring and visualization dashboards.
- **Health Check Endpoint**: Detailed JSON health reports available at `/health` tracking status for PostgreSQL, Redis, and Storage/S3.
- **Structured Logging**: Application logging powered by Serilog (`Serilog.AspNetCore`) with console output and daily rolling file sinks.

## Deployment & Infrastructure

- **Docker Containerization**: Multi-stage Dockerfile (`AutoHub.API/Dockerfile`) creating lightweight container builds.
- **Docker Compose Orchestration**: Environment configurations for local development (`docker-compose.local.yml`), production (`docker-compose.prod.yml`), and base services (`docker-compose.yml`).
- **GitHub Actions CI/CD**: Automated pipeline (`deploy.yml`) performing solution restore, build, test, SSH deployment to AWS EC2, and health checks.
- **AWS EC2 Deployment**: Deployment execution target hosted on Amazon Web Services (AWS) EC2.
- **AWS S3 Integration**: Cloud object storage integration using `AWSSDK.S3`.
- **PostgreSQL Database**: Relational database persistence using PostgreSQL 17 with Entity Framework Core 10 (`Npgsql.EntityFrameworkCore.PostgreSQL`).

## Developer Experience

- **Interactive Swagger Documentation**: OpenAPI documentation with Swashbuckle (`Swashbuckle.AspNetCore`) including JWT Bearer authorization support at `/swagger`.
- **Clean Architecture**: Clean/Onion Architecture separation into API, Application, Domain, and Infrastructure projects.
- **Modular Service Configuration**: Dependency injection setup organized through custom extension methods (`AddApplicationServices`, `AddJwtAuthentication`, `AddRateLimiterService`, `AddAwsServices`).
- **Environment Configuration**: Flexible app settings configuration (`appsettings.json`, `appsettings.Development.json`) supporting environment variable overrides.
