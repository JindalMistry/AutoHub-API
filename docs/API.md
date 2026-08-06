# AutoHub API Documentation

This document provides a comprehensive list of all API endpoints exposed by the AutoHub application. 

The application utilizes ASP.NET Core with JWT Bearer authentication and role-based access control (RBAC).

## Endpoints Reference Table

| HTTP Method | Route | Controller | Authentication Required (Yes/No) | Allowed Roles or Policies | One-line Description |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **GET** | `/api/admin/dashboard` | `AdminController` | Yes | `Admin` | Retrieves dashboard statistics and overview data. |
| **GET** | `/api/admin/top-vehicles` | `AdminController` | Yes | `Admin` | Retrieves analytics data for the top-performing vehicles. |
| **GET** | `/api/admin/top-dealers` | `AdminController` | Yes | `Admin` | Retrieves analytics data for the top-performing dealers. |
| **GET** | `/api/admin/pending-vehicles` | `AdminController` | Yes | `Admin` | Retrieves a list of vehicles currently in Draft status. |
| **GET** | `/api/admin/pending-dealers` | `AdminController` | Yes | `Admin` | Retrieves pending dealer applications. |
| **GET** | `/api/admin/completed-reservations` | `AdminController` | Yes | `Admin` | Retrieves completed and active reservations. |
| **GET** | `/api/analytics/trending` | `AnalyticsController` | No | None | Retrieves a list of trending vehicles. |
| **POST** | `/api/auth/register` | `AuthController` | No | None | Registers a new user. |
| **POST** | `/api/auth/login` | `AuthController` | No | None | Authenticates user credentials and issues a JWT token. |
| **POST** | `/api/jobs/recalculate-trending` | `BackgroundJobController` | No | None | Recalculates vehicle trending scores. |
| **POST** | `/api/jobs/expire-reservations` | `BackgroundJobController` | No | None | Scans and expires expired pending reservations. |
| **GET** | `/api/cache/cache-test` | `CacheController` | No | None | Performs a read/write test against the Redis cache. |
| **POST** | `/api/dealers/apply` | `DealersController` | Yes | Any Authenticated | Submits a dealer application profile. |
| **PUT** | `/api/dealers/{id}/approve` | `DealersController` | Yes | `Admin` | Approves a dealer application by their dealer profile ID. |
| **GET** | `/api/dealers/pending` | `DealersController` | Yes | `Admin` | Retrieves all pending dealer applications. |
| **PUT** | `/api/dealers/{id}/reject` | `DealersController` | Yes | `Admin` | Rejects a dealer application by their dealer profile ID. |
| **GET** | `/api/DeploymentTest` | `DeploymentTestController` | No | None | Retrieves system metadata to verify active deployment. |
| **POST** | `/api/favourites` | `FavouriteController` | Yes | `Buyer` | Adds a vehicle to the current buyer's favorites list. |
| **DELETE** | `/api/favourites` | `FavouriteController` | Yes | `Buyer` | Removes a vehicle from the current buyer's favorites list. |
| **GET** | `/api/favourites` | `FavouriteController` | Yes | `Buyer` | Retrieves a paginated list of the current buyer's favorite vehicles. |
| **GET** | `/api/Health` | `HealthController` | No | None | Performs a basic application health check. |
| **POST** | `/api/inquiry` | `InquiryController` | Yes | `Buyer` | Submits a new vehicle inquiry. |
| **GET** | `/api/inquiry/my` | `InquiryController` | Yes | `Buyer` | Retrieves all inquiries submitted by the current buyer. |
| **GET** | `/api/inquiry/{inquiryId}` | `InquiryController` | Yes | `Buyer`, `Dealer` | Retrieves details of a specific inquiry by its ID. |
| **GET** | `/api/inquiry/dealer` | `InquiryController` | Yes | `Dealer` | Retrieves all inquiries directed to the authenticated dealer's vehicles. |
| **PUT** | `/api/inquiry/{inquiryId}` | `InquiryController` | Yes | `Dealer` | Updates an inquiry's status or response by ID. |
| **POST** | `/api/reservation` | `ReservationController` | Yes | `Buyer` | Reserves a specific vehicle for purchase. |
| **DELETE** | `/api/reservation/{reservationId}` | `ReservationController` | Yes | `Admin` | Cancels an existing reservation. |
| **GET** | `/api/reservation/dealer/my` | `ReservationController` | Yes | `Dealer` | Retrieves reservations received by the authenticated dealer. |
| **GET** | `/api/reservation/my` | `ReservationController` | Yes | `Buyer` | Retrieves reservations made by the authenticated buyer. |
| **GET** | `/api/reservation/{reservationId}` | `ReservationController` | Yes | Any Authenticated | Retrieves a reservation's details by ID. |
| **POST** | `/api/storage/upload` | `StorageController` | No | None | Uploads a file to the S3/MinIO storage provider. |
| **GET** | `/api/auth-test` | `TestAuthController` | Yes | Any Authenticated | Test endpoint to verify global JWT token authorization. |
| **GET** | `/api/auth-test/admin` | `TestAuthController` | Yes | `Admin` | Test endpoint to verify Admin role authorization. |
| **GET** | `/api/test` | `TestController` | No | None | Test endpoint that returns a sample hashed password. |
| **POST** | `/api/vehicle-images/{vehicleId}/images` | `VehicleImagesController` | Yes | `Dealer` | Uploads files and attaches them as images to a specific vehicle. |
| **GET** | `/api/vehicle-images/{vehicleId}/images` | `VehicleImagesController` | No | None | Retrieves all images associated with a specific vehicle. |
| **DELETE** | `/api/vehicle-images/{imageId}` | `VehicleImagesController` | Yes | `Dealer` | Deletes a specific vehicle image from the catalog and storage. |
| **POST** | `/api/vehicles/add` | `VehiclesController` | Yes | `Dealer` | Creates a new vehicle listing draft. |
| **GET** | `/api/vehicles/my` | `VehiclesController` | Yes | `Dealer` | Retrieves vehicles listed by the current dealer. |
| **GET** | `/api/vehicles/{vehicleId}` | `VehiclesController` | No | None | Retrieves detailed specifications of a specific vehicle. |
| **GET** | `/api/vehicles/{vehicleId}/dealer` | `VehiclesController` | Yes | `Dealer`, `Admin` | Allows a dealer or admin to retrieve details of any vehicle. |
| **PUT** | `/api/vehicles/{vehicleId}` | `VehiclesController` | Yes | `Dealer` | Updates specifications or details of a vehicle listing. |
| **DELETE** | `/api/vehicles/{vehicleId}` | `VehiclesController` | Yes | `Dealer` | Deletes a vehicle listing from the catalog. |
| **PUT** | `/api/vehicles/{vehicleId}/publish` | `VehiclesController` | Yes | `Admin` | Publishes a dealer's vehicle listing draft. |
| **PUT** | `/api/vehicles/{vehicleId}/unpublish` | `VehiclesController` | Yes | `Admin` | Unpublishes a vehicle, changing its status back to draft. |
| **GET** | `/api/vehicles` | `VehiclesController` | No | None | Searches, filters, and paginates published vehicle listings. |
| **GET** | `/api/vehicles/filter-options` | `VehiclesController` | No | None | Retrieves all valid filter options for search sidebar inputs. |

---

## Non-Controller Mapped Endpoints

For completeness, the following endpoints are mapped within the application lifecycle (e.g. inside `Program.cs`) rather than via API controllers:

| HTTP Method | Route | Component/Service | Authentication Required (Yes/No) | Allowed Roles or Policies | One-line Description |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **GET** | `/health` | ASP.NET Core Health Checks | No | None | Returns JSON containing Postgres, Redis, and S3 status. |
| **GET** | `/metrics` | Prometheus Metrics | No | None | Exposes Prometheus-formatted metrics of HTTP requests and system status. |
| **GET/POST** | `/hangfire` | Hangfire Dashboard | Yes | Basic Auth (Credentials in app settings) | Provides a background job management UI dashboard. |
