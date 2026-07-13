# API Documentation for AutoHub

This document summarizes all API endpoints in the solution for frontend integration.

---

# Authentication Overview

- Login endpoint: POST /api/auth/login
- Registration endpoint: POST /api/auth/register
- JWT structure: standard JWT with claims: `sub` (NameIdentifier user id), `email`, `role`. Token signed with HMAC-SHA256 using `JwtSettings.Secret`. Issuer/Audience from `JwtSettings`.
- Refresh token endpoint: Not implemented in this codebase.
- Roles available: `Buyer`, `Dealer`, `Admin` (see `AutoHub.Domain.Enums.UserRole`).
- Authorization policies: No custom policy attributes found; controllers use `Authorize` with optional `Roles`.

---

# Common Types

**Response envelope**: All successful responses use `ApiResponse<T>` (AutoHub.Application.Common.ApiResponse):

- `Success` (bool)
- `Message` (string)
- `Data` (T | null)

**Error envelope**: `ErrorResponse`:

- `Success` = false
- `Message` (string)

---

# Controllers & Endpoints

Note: For each endpoint the `Authorization` section states whether auth is required and the roles.

---

## Auth - POST /api/auth/register

## Endpoint Information

- HTTP Method: POST
- Route URL: /api/auth/register
- Controller Name: AuthController
- Action Name: Register
- Description: Register a new user account.

## Authorization

- Authentication Required: No
- Roles Allowed: None
- Policies Required: None
- Anonymous Access Allowed: Yes

## Request Headers

| Header | Required | Description |
| - | - | - |
| Content-Type | Yes | application/json |

## Route Parameters

None

## Query Parameters

None

## Request Body

RegisterRequest:

| Property | Type | Required | Nullable | Description |
| - | - | -: | - | - |
| Name | string | Yes | No | Full name of the user |
| Email | string | Yes | No | Email address |
| Password | string | Yes | No | Plain text password (hashed server-side) |
| Role | enum (Buyer, Dealer, Admin) | Yes | No | User role |

## Example Request Body

```json
{
  "name": "Alice Example",
  "email": "alice@example.com",
  "password": "Password123",
  "role": "Buyer"
}
```

## Success Response

200 OK - ApiResponse<object>

| Property | Type | Nullable | Description |
| - | - | - | - |
| Success | bool | No | true |
| Message | string | No | confirmation message |
| Data | object | Yes | null |

## Example Success Response

```json
{
  "success": true,
  "message": "User registered successfully.",
  "data": null
}
```

## Error Responses

- 400 Bad Request — e.g. email already exists (ApiResponse/ErrorResponse)
- 500 Internal Server Error — unexpected

## Validation Rules

- No FluentValidation or DataAnnotation validators found for `RegisterRequest` in repository.

## Frontend Notes

- No pagination. Simple registration flow.

---

## Auth - POST /api/auth/login

### Endpoint Information

- HTTP Method: POST
- Route URL: /api/auth/login
- Controller Name: AuthController
- Action Name: Login
- Description: Authenticate user and return JWT token and metadata.

### Authorization

- Authentication Required: No
- Roles Allowed: None
- Policies Required: None
- Anonymous Access Allowed: Yes

### Request Headers

| Header | Required | Description |
| - | - | - |
| Content-Type | Yes | application/json |

### Request Body

LoginRequest:

| Property | Type | Required | Nullable | Description |
| - | - | -: | - | - |
| Email | string | Yes | No | User email |
| Password | string | Yes | No | Plain text password |

### Example Request Body

```json
{
  "email": "alice@example.com",
  "password": "Password123"
}
```

### Success Response

200 OK - ApiResponse<AuthResponse>

AuthResponse:

| Property | Type | Nullable | Description |
| - | - | - | - |
| Token | string | No | JWT token |
| ExpiresAt | string (ISO datetime) | No | Expiration datetime (UTC) |
| Email | string | No | User email |
| Role | string | No | Role name |

### Example Success Response

```json
{
  "success": true,
  "message": "Login successful.",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6...",
    "expiresAt": "2026-06-21T12:34:56Z",
    "email": "alice@example.com",
    "role": "Buyer"
  }
}
```

### Error Responses

- 400 Bad Request — invalid password (returns BadRequestException)
- 404 Not Found — user not found
- 500 Internal Server Error

### Validation Rules

- No explicit validators found for `LoginRequest` in repository.

### Frontend Notes

- Use the returned `token` in `Authorization: Bearer <token>` header for protected endpoints.

---

## VehiclesController (Route prefix: /api/vehicles)

### POST /api/vehicles/add

- HTTP Method: POST
- Route: /api/vehicles/add
- Controller: VehiclesController
- Action: Create
- Description: Dealer creates a vehicle listing.

Authorization:
- Authentication Required: Yes
- Roles Allowed: Dealer
- Anonymous Access Allowed: No

Request Headers:
| Header | Required | Description |
| - | - | - |
| Authorization | Yes | Bearer token |
| Content-Type | Yes | application/json |

Route Parameters: none

Request Body: CreateVehicleRequest

| Property | Type | Required | Nullable | Description |
| - | - | -: | - | - |
| Title | string | Yes | No | Listing title |
| RegNo | string | Yes | No | Registration number |
| Price | decimal | Yes | No | Price |
| Make | string | Yes | No | Make |
| Model | string | Yes | No | Model |
| Variant | string | Yes | No | Variant |
| Year | int | Yes | No | Year built |
| Mileage | int | Yes | No | Mileage |
| FuelType | enum | Yes | No | FuelType enum |
| Transmission | enum | Yes | No | TransmissionType enum |
| Description | string | Yes | No | Details |

Example Request Body:
```json
{
  "title": "2018 Honda Civic",
  "regNo": "ABC123",
  "price": 12000.00,
  "make": "Honda",
  "model": "Civic",
  "variant": "EX",
  "year": 2018,
  "mileage": 45000,
  "fuelType": "Petrol",
  "transmission": "Automatic",
  "description": "Well maintained."
}
```

Success Response:
- 200 OK - ApiResponse<VehicleResponse>
VehicleResponse fields: Id, Title, Price, Status, Make, Model, Variant

Example:
```json
{
  "success": true,
  "message": "Vehicle created successfully",
  "data": { "id": "...", "title": "2018 Honda Civic", "price": 12000.0, "status": "Draft", "make": "Honda", "model": "Civic", "variant": "EX" }
}
```

Error Responses: 400, 401, 403, 500

Validation: No explicit validators found.

Frontend Notes: After create, client may call upload images endpoint for the vehicle.

---

### GET /api/vehicles/my

- HTTP Method: GET
- Route: /api/vehicles/my
- Auth: Dealer
- Description: Get all vehicles created by authenticated dealer.

Headers: Authorization required
Query params: none

Success Response: ApiResponse<List<VehicleResponse>>

Example Data: array of VehicleResponse.

---

### GET /api/vehicles/{vehicleId}

- HTTP Method: GET
- Route: /api/vehicles/{vehicleId}
- Auth: No (AllowAnonymous)
- Route Parameter: `vehicleId` (Guid)
- Description: Get one vehicle by id

Success Response: ApiResponse<VehicleResponse>

---

### PUT /api/vehicles/{vehicleId}

- HTTP Method: PUT
- Route: /api/vehicles/{vehicleId}
- Auth: Dealer
- Request Body: CreateVehicleRequest
- Description: Update vehicle

Success Response: ApiResponse<VehicleResponse>

---

### DELETE /api/vehicles/{vehicleId}

- HTTP Method: DELETE
- Route: /api/vehicles/{vehicleId}
- Auth: Dealer
- Description: Delete vehicle

Success Response: ApiResponse<object> (message)

---

### PUT /api/vehicles/{vehicleId}/publish

- HTTP Method: PUT
- Route: /api/vehicles/{vehicleId}/publish
- Auth: Admin
- Description: Publish vehicle (make visible)

Success Response: ApiResponse<object>

---

### PUT /api/vehicles/{vehicleId}/unpublish

- HTTP Method: PUT
- Route: /api/vehicles/{vehicleId}/unpublish
- Auth: Admin
- Description: Move vehicle to draft

Success Response: ApiResponse<object>

---

### GET /api/vehicles

- HTTP Method: GET
- Route: /api/vehicles
- Auth: AllowAnonymous
- Description: Search and list vehicles with pagination and sorting

Query Parameters (VehicleSearchRequest):

| Parameter | Type | Required | Default Value | Description |
| - | - | - | - | - |
| SearchTerm | string | No | null | Full-text search term |
| Make | string | No | null | Filter by make |
| Model | string | No | null | Filter by model |
| Variant | string | No | null | Filter by variant |
| MinPrice | decimal | No | null | Minimum price |
| MaxPrice | decimal | No | null | Maximum price |
| MinYear | int | No | null | Minimum year |
| MaxYear | int | No | null | Maximum year |
| FuelType | enum | No | null | Fuel type |
| Transmission | enum | No | null | Transmission type |
| SortBy | enum | No | Newest | Sorting (VehicleSortBy)
| PageNumber | int | No | 1 |
| PageSize | int | No | 10 |

Success Response: ApiResponse<PaginatedResponse<VehicleListingResponse>>

PaginatedResponse<T> fields: Items[], PageNumber, PageSize, TotalRecords, TotalPages

Example response data (simplified):
```json
{
  "success": true,
  "message": "Vehicles retrieved successfully.",
  "data": {
    "items": [ { "id": "...", "title": "2018 Honda Civic", "make": "Honda", "model": "Civic", "variant": "EX", "price": 12000.0 } ],
    "pageNumber": 1,
    "pageSize": 10,
    "totalRecords": 42,
    "totalPages": 5
  }
}
```

Frontend Notes:
- This endpoint is paginated and supports filtering, sorting and searching via the query parameters.

---

### GET /api/vehicles/filter-options

- HTTP Method: GET
- Route: /api/vehicles/filter-options
- Auth: AllowAnonymous
- Description: Returns available filter options (makes, models, variants, fuel types, transmissions, price range)

Response: ApiResponse<VehicleFilterOptionsResponse>

VehicleFilterOptionsResponse fields:
- Makes: string[]
- Models: string[]
- Variants: string[]
- FuelTypes: EnumOptionResponse[] { value:int, name:string }
- Transmissions: EnumOptionResponse[]
- MinPrice: decimal
- MaxPrice: decimal

Example:
```json
{
  "success": true,
  "message": "Filter options retrieved successfully.",
  "data": {
    "makes": ["Honda","Toyota"],
    "models": ["Civic","Corolla"],
    "variants": ["EX","LX"],
    "fuelTypes": [ { "value": 0, "name": "Petrol" } ],
    "transmissions": [ { "value": 0, "name": "Manual" } ],
    "minPrice": 1000,
    "maxPrice": 999999
  }
}
```

---

## VehicleImagesController (Route: /api/vehicle-images)

### POST /api/vehicle-images/{vehicleId}/images

- HTTP Method: POST
- Route: /api/vehicle-images/{vehicleId}/images
- Auth: Dealer
- Consumes: multipart/form-data
- Description: Upload one or more images for a vehicle

Headers:
| Header | Required | Description |
| - | - | - |
| Authorization | Yes | Bearer token |
| Content-Type | Yes | multipart/form-data |

Route Parameters:
- `vehicleId` (Guid) required

Request Body (form): UploadVehicleImagesRequest
- Files: array of file fields

Example (multipart/form-data): field `files` with multiple files

Success Response: ApiResponse<List<VehicleImageResponse>>

VehicleImageResponse: Id, ImageUrl, DisplayOrder

---

### GET /api/vehicle-images/{vehicleId}/images

- HTTP Method: GET
- Route: /api/vehicle-images/{vehicleId}/images
- Auth: Authorized (any authenticated user)
- Description: Get images for a vehicle

Success Response: ApiResponse<List<VehicleImageResponse>>

---

### DELETE /api/vehicle-images/{imageId}

- HTTP Method: DELETE
- Route: /api/vehicle-images/{imageId}
- Auth: Dealer
- Description: Delete an image by id

Success Response: ApiResponse<object>

---

## StorageController

### POST /api/storage/upload

- HTTP Method: POST
- Route: /api/storage/upload
- Consumes: multipart/form-data
- Auth: No
- Description: Upload a file to storage service

Request Body: UploadFileRequest (form) with `file` IFormFile

Success Response: ApiResponse<string> where Data = uploaded object name/path

Example Response Data: `"bucket/objectname.jpg"`

---

## ReservationController (Route: /api/reservation)

### POST /api/reservation

- HTTP Method: POST
- Auth: Buyer
- Request Body: CreateReservationRequest { vehicleId: GUID }
- Success Response: ApiResponse<CreateReservationResponse> (Id)

### DELETE /api/reservation/{reservationId}

- HTTP Method: DELETE
- Auth: Admin
- Route param: reservationId (GUID)
- Success Response: ApiResponse<object>

### GET /api/reservation/dealer/my

- HTTP Method: GET
- Auth: Dealer
- Query parameters: Status (ReservationStatus?), PageNumber=1, Pagesize=20
- Success: ApiResponse<PaginatedResponse<ReservationResponse>>

### GET /api/reservation/my

- HTTP Method: GET
- Auth: Buyer
- Query: Status (ReservationStatus?)
- Success: ApiResponse<List<ReservationResponse>>

### GET /api/reservation/{reservationId}

- HTTP Method: GET
- Auth: Authorized
- Success: ApiResponse<ReservationResponse>

ReservationResponse fields summary: Id, VehicleId, Vehicle (VehicleListingResponse), Status, ExpiresAt, CreatedAt, BuyerName, BuyerEmail, BuyerPhone

---

## InquiryController (Route: /api/inquiry)

### POST /api/inquiry

- HTTP Method: POST
- Auth: Buyer
- Request: CreateInquiryRequest { DealerId?:GUID, VehicleId?:GUID, InquiryType, Message }
- Success: ApiResponse<object>

### GET /api/inquiry/my

- HTTP Method: GET
- Auth: Buyer
- Query: InquirySearchRequest { Status?, InquiryType?, PageNumber=1, PageSize=10 }
- Success: ApiResponse<PaginatedResponse<InquiryResponse>>

### GET /api/inquiry/{inquiryId}

- HTTP Method: GET
- Auth: Buyer,Dealer
- Route param: inquiryId GUID
- Success: ApiResponse<InquiryResponse>

### GET /api/inquiry/dealer

- HTTP Method: GET
- Auth: Dealer
- Query: InquirySearchRequest
- Success: ApiResponse<PaginatedResponse<InquiryResponse>>

### PUT /api/inquiry/{inquiryId}

- HTTP Method: PUT
- Auth: Dealer
- Request: UpdateInquiryRequest { Status, DealerMessage }
- Success: ApiResponse<object>

InquiryResponse fields summary: Id, VehicleId, VehicleTitle, InquiryType, Status, Message, DealerMessage, CreatedAt, BuyerName

---

## HealthController

### GET /api/health

- HTTP Method: GET
- Route: /api/health
- Auth: No (no attribute; public)
- Description: Health check. Returns string "healthy"

---

## FavouriteController (Route: /api/favourites)

### POST /api/favourites
- HTTP Method: POST
- Auth: Buyer
- Request: AddFavouriteRequest { VehicleId }
- Success: ApiResponse<object>

### DELETE /api/favourites
- HTTP Method: DELETE
- Auth: Buyer
- Request: RemoveFavouriteRequest { VehicleId }
- Success: ApiResponse<object>

### GET /api/favourites
- HTTP Method: GET
- Auth: Buyer
- Query: pageNumber=1,pageSize=10
- Success: ApiResponse<PaginatedResponse<VehicleListingResponse>>

---

## DealersController (Route: /api/dealers)

### POST /api/dealers/apply
- HTTP Method: POST
- Auth: Authorized (any authenticated user)
- Request: CreateDealerProfileRequest { BusinessName, Phone, Country, City, Pincode }
- Success: ApiResponse<DealerResponse>

### PUT /api/dealers/{id}/approve
- HTTP Method: PUT
- Auth: Admin
- Route param: id GUID
- Success: ApiResponse<object>

### GET /api/dealers/pending
- HTTP Method: GET
- Auth: Admin
- Success: ApiResponse<List<DealerResponse>>

### PUT /api/dealers/{id}/reject
- HTTP Method: PUT
- Auth: Admin
- Success: ApiResponse<object>

---

## CacheController

### GET /api/cache/cache-test
- HTTP Method: GET
- Auth: No
- Description: Test cache set/get. Returns cached string value.

---

## BackgroundJobController (Route: /api/jobs)

### POST /api/jobs/recalculate-trending
- HTTP Method: POST
- Auth: No (no Authorize attribute present)
- Description: Triggers background job to recalculate trending scores.

### POST /api/jobs/expire-reservations
- HTTP Method: POST
- Auth: No
- Description: Triggers reservation expiry job.

---

## AnalyticsController

### GET /api/analytics/trending
- HTTP Method: GET
- Route: /api/analytics/trending
- Auth: AllowAnonymous
- Success: ApiResponse<List<VehicleListingResponse>>

---

## AdminController (Route: /api/admin)

### GET /api/admin/dashboard
- HTTP Method: GET
- Auth: Admin
- Success: ApiResponse<AdminDashboardResponse>

### GET /api/admin/top-vehicles
- HTTP Method: GET
- Auth: Admin
- Success: ApiResponse<List<TopVehiclesResponse>>

### GET /api/admin/top-dealers
- HTTP Method: GET
- Auth: Admin
- Success: ApiResponse<List<TopDealerResponse>>

### GET /api/admin/pending-vehicles
- HTTP Method: GET
- Auth: Admin
- Success: ApiResponse<List<VehicleListingResponse>>

### GET /api/admin/pending-dealers
- HTTP Method: GET
- Auth: Admin
- Success: ApiResponse<List<DealerResponse>>

### GET /api/admin/completed-reservations
- HTTP Method: GET
- Auth: Admin
- Success: ApiResponse<List<ReservationResponse>>

---

# API Summary Table

| Method | Route | Auth Required | Roles | Description |
| - | - | - | - | - |
| POST | /api/auth/register | No | - | Register new user |
| POST | /api/auth/login | No | - | Login and return JWT |
| POST | /api/vehicles/add | Yes | Dealer | Create vehicle |
| GET | /api/vehicles/my | Yes | Dealer | Dealer's vehicles |
| GET | /api/vehicles/{vehicleId} | No | - | Get vehicle by id |
| PUT | /api/vehicles/{vehicleId} | Yes | Dealer | Update vehicle |
| DELETE | /api/vehicles/{vehicleId} | Yes | Dealer | Delete vehicle |
| PUT | /api/vehicles/{vehicleId}/publish | Yes | Admin | Publish vehicle |
| PUT | /api/vehicles/{vehicleId}/unpublish | Yes | Admin | Unpublish vehicle |
| GET | /api/vehicles | No | - | Search vehicles (paginated) |
| GET | /api/vehicles/filter-options | No | - | Filter options |
| POST | /api/vehicle-images/{vehicleId}/images | Yes | Dealer | Upload images |
| GET | /api/vehicle-images/{vehicleId}/images | Yes | Any auth | Get vehicle images |
| DELETE | /api/vehicle-images/{imageId} | Yes | Dealer | Delete image |
| POST | /api/storage/upload | No | - | Upload file to storage |
| POST | /api/reservation | Yes | Buyer | Create reservation |
| DELETE | /api/reservation/{reservationId} | Yes | Admin | Cancel reservation |
| GET | /api/reservation/dealer/my | Yes | Dealer | Dealer reservations (paginated) |
| GET | /api/reservation/my | Yes | Buyer | Buyer reservations |
| GET | /api/reservation/{reservationId} | Yes | Any auth | Get reservation by id |
| POST | /api/inquiry | Yes | Buyer | Create inquiry |
| GET | /api/inquiry/my | Yes | Buyer | My inquiries (paginated) |
| GET | /api/inquiry/{inquiryId} | Yes | Buyer,Dealer | Get inquiry by id |
| GET | /api/inquiry/dealer | Yes | Dealer | Dealer inquiries (paginated) |
| PUT | /api/inquiry/{inquiryId} | Yes | Dealer | Update an inquiry |
| GET | /api/health | No | - | Health check |
| POST | /api/favourites | Yes | Buyer | Add favourite |
| DELETE | /api/favourites | Yes | Buyer | Remove favourite |
| GET | /api/favourites | Yes | Buyer | Get favourites (paginated) |
| POST | /api/dealers/apply | Yes | Any auth | Apply to be dealer |
| PUT | /api/dealers/{id}/approve | Yes | Admin | Approve dealer |
| GET | /api/dealers/pending | Yes | Admin | Pending dealers |
| PUT | /api/dealers/{id}/reject | Yes | Admin | Reject dealer |
| GET | /api/cache/cache-test | No | - | Cache test |
| POST | /api/jobs/recalculate-trending | No | - | Trigger trending job |
| POST | /api/jobs/expire-reservations | No | - | Trigger reservation expiry job |
| GET | /api/analytics/trending | No | - | Trending vehicles |
| GET | /api/admin/dashboard | Yes | Admin | Admin dashboard data |
| GET | /api/admin/top-vehicles | Yes | Admin | Top vehicles |
| GET | /api/admin/top-dealers | Yes | Admin | Top dealers |
| GET | /api/admin/pending-vehicles | Yes | Admin | Pending vehicles |
| GET | /api/admin/pending-dealers | Yes | Admin | Pending dealers |
| GET | /api/admin/completed-reservations | Yes | Admin | Completed reservations |

---

# Validation Summary

- No FluentValidation or DataAnnotation validators were found in the repository for DTOs; validation appears to be handled in services or not implemented.

# Error Handling

Common HTTP errors returned by services:
- 400 Bad Request — validation or business errors (BadRequestException)
- 401 Unauthorized — missing/invalid token
- 403 Forbidden — role-based access denied
- 404 Not Found — missing resource
- 500 Internal Server Error — unexpected exceptions

Example 401 header usage for protected endpoints:

```
Authorization: Bearer <JWT_TOKEN>
Content-Type: application/json
```

---

# Implementation Notes for Frontend

- Use `POST /api/auth/login` to obtain token; store and attach `Authorization: Bearer <token>` to protected requests.
- Endpoints that return paginated data use `PageNumber` and `PageSize`.
- `GET /api/vehicles` supports filtering, sorting, searching via query string matching `VehicleSearchRequest` properties.
- Image upload endpoints require `multipart/form-data` with file fields named `files` (multiple allowed).
- Where route parameters are GUIDs, send them as standard UUID strings.

---

If you want, I can:
- Add sample curl commands for common flows (login + search),
- Generate an OpenAPI spec (Swagger) from these controllers,
- Or produce TypeScript interfaces for DTOs.

