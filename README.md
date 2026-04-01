# NiagaOne

A full-stack logistics management platform built with **.NET 8**, **ASP.NET Core Web API**, **Blazor**, and **MySQL** — following **Clean Architecture** principles.

NiagaOne provides end-to-end shipment lifecycle management including warehouse operations, driver and vehicle fleet management, real-time shipment tracking, and a granular role-based access control (RBAC) system with JWT authentication.

---

## Table of Contents

- [Project Background](#project-background)
- [Key Features](#key-features)
- [System Architecture](#system-architecture)
- [Tech Stack](#tech-stack)
- [Project Structure](#project-structure)
- [Domain Model](#domain-model)
- [API Reference](#api-reference)
- [Authentication & Authorization](#authentication--authorization)
- [Getting Started](#getting-started)
- [Configuration](#configuration)
- [Default Accounts](#default-accounts)
- [License](#license)

---

## Project Background

NiagaOne was designed to solve the core operational challenges in logistics management — tracking shipments from origin warehouse to destination, managing a fleet of drivers and vehicles, and enforcing access control across different organizational roles.

The platform addresses:

- **Shipment lifecycle management** — from creation through assignment, transit tracking, and delivery confirmation
- **Fleet coordination** — registering and managing drivers and vehicles with status tracking
- **Warehouse operations** — managing origin warehouses and their capacities
- **Access control** — fine-grained, permission-based authorization ensuring users only access what their role allows
- **Multi-client architecture** — a decoupled REST API consumed by a Blazor WebAssembly SPA, ready to support additional clients (mobile, third-party integrations)

---

## Key Features

- Full CRUD operations for shipments, drivers, vehicles, and warehouses
- Unique tracking number generation for every shipment
- Shipment assignment workflow (driver + vehicle pairing)
- Real-time shipment tracking with location and status history
- JWT authentication with refresh token rotation and revocation
- RBAC with 4 built-in roles and 18 granular permissions
- Custom role creation with flexible permission assignment
- Blazor Server + WebAssembly hybrid frontend
- Auto-migration on application startup
- Swagger/OpenAPI documentation

---

## System Architecture

NiagaOne follows **Clean Architecture** (also known as Onion Architecture), enforcing strict dependency boundaries where all dependencies point inward.

```
┌──────────────────────────────────────────────────────────────────┐
│                        Presentation Layer                        │
│  ┌─────────────────────────┐    ┌─────────────────────────────┐  │
│  │    ASP.NET Core API     │    │   Blazor Server + WASM      │  │
│  │  Controllers / Filters  │    │   Pages / Services          │  │
│  └────────────┬────────────┘    └──────────────┬──────────────┘  │
│               │         HTTP (REST API)        │                 │
│               │◄───────────────────────────────┘                 │
├───────────────┼──────────────────────────────────────────────────┤
│               ▼          Application Layer                       │
│  ┌──────────────────────────────────────────────────────────┐    │
│  │   Use Cases  ·  DTOs  ·  Interfaces (Ports)              │    │
│  └──────────────────────────┬───────────────────────────────┘    │
├─────────────────────────────┼────────────────────────────────────┤
│               ▼          Infrastructure Layer                    │
│  ┌──────────────────────────────────────────────────────────┐    │
│  │  EF Core Repositories  ·  JWT Service  ·  BCrypt Hasher  │    │
│  │  AppDbContext  ·  Migrations  ·  Current User Service     │    │
│  └──────────────────────────┬───────────────────────────────┘    │
├─────────────────────────────┼────────────────────────────────────┤
│               ▼            Domain Layer                          │
│  ┌──────────────────────────────────────────────────────────┐    │
│  │  Entities  ·  Enums  ·  Value Objects                     │    │
│  │  (Zero external dependencies)                             │    │
│  └──────────────────────────────────────────────────────────┘    │
└──────────────────────────────────────────────────────────────────┘
                              │
                              ▼
                    ┌──────────────────┐
                    │    MySQL 8.0     │
                    │  niagaone   │
                    └──────────────────┘
```

### Layer Responsibilities

| Layer | Project | Allowed Dependencies | Responsibility |
|---|---|---|---|
| **Domain** | `Domain/` | None | Entities, enums — pure business model |
| **Application** | `Application/` | Domain | Interfaces (ports), DTOs, use cases — orchestration |
| **Infrastructure** | `Infrastructure/` | Domain + Application | EF Core, repositories (adapters), external services |
| **API** | `API/` | Application + Infrastructure | Controllers, middleware, DI composition root |
| **Frontend** | `BlazorApp/` | — | Blazor Server host + WebAssembly client SPA |

---

## Tech Stack

| Component | Technology | Version |
|---|---|---|
| Runtime | .NET | 8.0 |
| Language | C# | 12 |
| API Framework | ASP.NET Core Web API | 8.0 |
| ORM | Entity Framework Core | 8.0.24 |
| Database | MySQL | 8.0 |
| MySQL Provider | Pomelo.EntityFrameworkCore.MySql | 8.0.2 |
| Authentication | JWT Bearer (HS256) | 8.0.14 |
| Password Hashing | BCrypt.Net-Next | 4.0.3 |
| API Docs | Swashbuckle (Swagger) | 6.6.2 |
| Frontend | Blazor Server + WebAssembly | 8.0.22 |
| Local Storage | Blazored.LocalStorage | 4.5.0 |

---

## Project Structure

```
NiagaOne/
├── Domain/                          # Domain Layer — entities & enums
│   ├── Entities/
│   │   ├── User.cs
│   │   ├── Driver.cs
│   │   ├── Vehicle.cs
│   │   ├── Warehouse.cs
│   │   ├── Shipment.cs
│   │   ├── ShipmentAssignment.cs
│   │   ├── ShipmentTracking.cs
│   │   ├── Role.cs
│   │   ├── Permission.cs
│   │   ├── UserRoleAssignment.cs
│   │   ├── RolePermission.cs
│   │   └── RefreshToken.cs
│   └── Enums/
│       └── StatusEnums.cs
│
├── Application/                     # Application Layer — use cases & contracts
│   ├── DTOs/
│   │   ├── Auth/                    # LoginRequest, LoginResponse, RegisterRequest, RefreshTokenRequest
│   │   ├── Users/                   # UserDto, UpdateUserRequest, AssignRoleRequest
│   │   ├── Roles/                   # RoleDto, PermissionDto, CreateRoleRequest
│   │   ├── Shipments/               # ShipmentDto, CreateShipmentRequest, TrackingEventDto
│   │   ├── Drivers/                 # DriverDto, CreateDriverRequest
│   │   ├── Vehicles/                # VehicleDto, CreateVehicleRequest
│   │   └── Warehouses/              # WarehouseDto, CreateWarehouseRequest
│   ├── Interfaces/                  # Repository & service contracts (ports)
│   └── UseCases/                    # Business logic orchestration
│       ├── Auth/AuthUseCase.cs
│       ├── Shipments/ShipmentUseCase.cs
│       ├── Users/UserUseCase.cs
│       ├── Roles/RoleUseCase.cs
│       ├── Drivers/DriverUseCase.cs
│       ├── Vehicles/VehicleUseCase.cs
│       └── Warehouses/WarehouseUseCase.cs
│
├── Infrastructure/                  # Infrastructure Layer — implementations
│   ├── Persistence/
│   │   └── AppDbContext.cs          # Fluent API config, seed data
│   ├── Migrations/                  # EF Core migration history
│   ├── Repositories/                # Data access implementations
│   ├── Services/
│   │   ├── JwtTokenService.cs       # JWT access + refresh token generation
│   │   ├── BCryptPasswordHasher.cs  # Password hashing (work factor 12)
│   │   └── CurrentUserService.cs    # Extract current user from HTTP context
│   └── DependencyInjection.cs       # Centralized service registration
│
├── API/                             # Presentation Layer — HTTP boundary
│   ├── Controllers/
│   │   ├── AuthController.cs        # Register, login, refresh, logout
│   │   ├── ShipmentsController.cs   # CRUD + assign + tracking
│   │   ├── UsersController.cs       # CRUD + role assignment
│   │   ├── RolesController.cs       # CRUD + permission management
│   │   ├── DriversController.cs     # CRUD
│   │   ├── VehiclesController.cs    # CRUD
│   │   └── WarehousesController.cs  # CRUD
│   ├── Filters/
│   │   └── RequirePermissionAttribute.cs  # Permission-based authorization
│   ├── Program.cs                   # App composition root
│   └── appsettings.json             # Configuration
│
├── BlazorApp/                       # Frontend — Blazor Server host
│   └── BlazorApp/
│       ├── Components/              # Server-side Razor components
│       ├── Services/                # Server auth state provider
│       └── Program.cs
│
├── BlazorApp/                       # Frontend — Blazor WebAssembly client
│   └── BlazorApp.Client/
│       ├── Pages/                   # Dashboard, Auth, Shipments, Drivers, etc.
│       ├── Layout/                  # AppLayout, AuthLayout
│       ├── Models/                  # Client-side models
│       ├── Services/                # ApiClient, AuthService, JwtAuthStateProvider
│       └── Shared/                  # AuthGuard, RedirectToLogin
│
└── NiagaOne.sln                  # Solution file
```

---

## Domain Model

### Entity Relationship Diagram

```
┌──────────┐       ┌──────────────────┐       ┌───────────┐
│   User   │1────N│ UserRoleAssignment │N────1│   Role    │
│──────────│       │──────────────────│       │───────────│
│ Id       │       │ UserId           │       │ Id        │
│ Name     │       │ RoleId           │       │ Name      │
│ Email    │       │ AssignedAt       │       │ IsSystem  │
│ Password │       │ AssignedBy       │       │           │
│ IsActive │       └──────────────────┘       └─────┬─────┘
└────┬─────┘                                        │1
     │1                                             │
     │                                    ┌─────────┴────────┐
     ├────N─┐                             │  RolePermission   │
     │      │                             │──────────────────│
┌────┴────┐ │                             │ RoleId           │
│ Driver  │ │                             │ PermissionId     │
│─────────│ │                             └────────┬─────────┘
│ Id      │ │                                      │N
│ License │ │                                      │
│ Phone   │ │                             ┌────────┴─────────┐
│ Status  │ │                             │   Permission     │
└────┬────┘ │                             │──────────────────│
     │      │                             │ Id               │
     │1     │N                            │ Name (e.g.       │
     │ ┌────┴──────────┐                  │  shipments.read) │
     │ │ RefreshToken   │                 │ Resource         │
     │ │───────────────│                  │ Action           │
     │ │ Id            │                  └──────────────────┘
     │ │ TokenHash     │
     │ │ ExpiresAt     │
     │ │ RevokedAt     │
     │ └───────────────┘
     │
     │1
┌────┴──────────────┐        ┌──────────────┐
│ ShipmentAssignment │N────1│   Vehicle     │
│───────────────────│        │──────────────│
│ Id                │        │ Id           │
│ ShipmentId        │        │ PlateNumber  │
│ DriverId          │        │ VehicleType  │
│ VehicleId         │        │ Capacity     │
│ AssignedAt        │        │ Status       │
└────────┬──────────┘        └──────────────┘
         │N
         │
         │1
┌────────┴─────────┐         ┌──────────────┐
│    Shipment      │N──────1│  Warehouse    │
│──────────────────│         │──────────────│
│ Id               │         │ Id           │
│ TrackingNumber   │         │ Name         │
│ OriginWarehouse  │         │ Location     │
│ Destination      │         │ Capacity     │
│ Weight / Volume  │         └──────────────┘
│ Status           │
└────────┬─────────┘
         │1
         │
         │N
┌────────┴─────────────┐
│  ShipmentTracking    │
│──────────────────────│
│ Id                   │
│ ShipmentId           │
│ Status               │
│ Location             │
│ Notes                │
│ CreatedAt            │
└──────────────────────┘
```

### Status Enums

| Entity | Possible Statuses |
|---|---|
| **Driver** | `Available`, `OnDuty`, `OffDuty`, `Suspended` |
| **Vehicle** | `Available`, `InUse`, `UnderMaintenance`, `Retired` |
| **Shipment** | `Pending`, `Assigned`, `PickedUp`, `InTransit`, `OutForDelivery`, `Delivered`, `Failed`, `Cancelled` |

---

## API Reference

Base URL: `http://localhost:5164/api`

### Authentication

| Method | Endpoint | Description | Auth |
|---|---|---|---|
| POST | `/api/auth/register` | Register a new user (auto-assigned Viewer role) | No |
| POST | `/api/auth/login` | Login and receive JWT + refresh token | No |
| POST | `/api/auth/refresh` | Rotate refresh token for new token pair | No |
| POST | `/api/auth/logout` | Revoke a specific refresh token | Yes |
| POST | `/api/auth/logout-all` | Revoke all refresh tokens for current user | Yes |

### Users

| Method | Endpoint | Description | Permission |
|---|---|---|---|
| GET | `/api/users` | List all users | `users.read` |
| GET | `/api/users/{id}` | Get user by ID | `users.read` |
| PUT | `/api/users/{id}` | Update user details | `users.update` |
| DELETE | `/api/users/{id}` | Delete user | `users.delete` |
| POST | `/api/users/{id}/roles` | Assign role to user | `roles.assign` |
| DELETE | `/api/users/{id}/roles/{roleId}` | Remove role from user | `roles.assign` |

### Roles & Permissions

| Method | Endpoint | Description | Permission |
|---|---|---|---|
| GET | `/api/roles` | List all roles with permissions | `roles.read` |
| GET | `/api/roles/{id}` | Get role by ID | `roles.read` |
| POST | `/api/roles` | Create custom role | `roles.create` |
| PUT | `/api/roles/{id}` | Update role (non-system only) | `roles.update` |
| DELETE | `/api/roles/{id}` | Delete role (non-system only) | `roles.delete` |
| PUT | `/api/roles/{id}/permissions` | Replace role permissions | `roles.update` |
| GET | `/api/permissions` | List all available permissions | `roles.read` |

### Shipments

| Method | Endpoint | Description | Permission |
|---|---|---|---|
| GET | `/api/shipments` | List all shipments | `shipments.read` |
| GET | `/api/shipments/{id}` | Get shipment by ID | `shipments.read` |
| GET | `/api/shipments/track/{trackingNumber}` | Track shipment by tracking number | `shipments.read` |
| POST | `/api/shipments` | Create shipment (auto-generates tracking number) | `shipments.create` |
| PUT | `/api/shipments/{id}` | Update shipment | `shipments.update` |
| DELETE | `/api/shipments/{id}` | Delete shipment (Pending/Cancelled only) | `shipments.delete` |
| POST | `/api/shipments/{id}/assign` | Assign driver and vehicle | `shipments.assign` |
| GET | `/api/shipments/{id}/tracking` | Get tracking history | `tracking.read` |
| POST | `/api/shipments/{id}/tracking` | Add tracking event | `tracking.create` |

### Drivers

| Method | Endpoint | Description | Permission |
|---|---|---|---|
| GET | `/api/drivers` | List all drivers | `drivers.manage` |
| GET | `/api/drivers/{id}` | Get driver by ID | `drivers.manage` |
| POST | `/api/drivers` | Register driver | `drivers.manage` |
| PUT | `/api/drivers/{id}` | Update driver | `drivers.manage` |
| DELETE | `/api/drivers/{id}` | Delete driver | `drivers.manage` |

### Vehicles

| Method | Endpoint | Description | Permission |
|---|---|---|---|
| GET | `/api/vehicles` | List all vehicles | `vehicles.manage` |
| GET | `/api/vehicles/{id}` | Get vehicle by ID | `vehicles.manage` |
| POST | `/api/vehicles` | Register vehicle (unique plate) | `vehicles.manage` |
| PUT | `/api/vehicles/{id}` | Update vehicle | `vehicles.manage` |
| DELETE | `/api/vehicles/{id}` | Delete vehicle | `vehicles.manage` |

### Warehouses

| Method | Endpoint | Description | Permission |
|---|---|---|---|
| GET | `/api/warehouses` | List all warehouses | `warehouses.manage` |
| GET | `/api/warehouses/{id}` | Get warehouse by ID | `warehouses.manage` |
| POST | `/api/warehouses` | Create warehouse | `warehouses.manage` |
| PUT | `/api/warehouses/{id}` | Update warehouse | `warehouses.manage` |
| DELETE | `/api/warehouses/{id}` | Delete warehouse | `warehouses.manage` |

---

## Authentication & Authorization

### JWT Authentication Flow

```
  Client                          API                         Database
    │                              │                              │
    │  POST /api/auth/login        │                              │
    │  { email, password }         │                              │
    │─────────────────────────────►│                              │
    │                              │  Verify password (BCrypt)    │
    │                              │─────────────────────────────►│
    │                              │  Load roles & permissions    │
    │                              │◄─────────────────────────────│
    │                              │  Generate JWT (15 min)       │
    │                              │  Generate refresh token (7d) │
    │                              │  Store refresh token hash    │
    │                              │─────────────────────────────►│
    │  { accessToken,              │                              │
    │    refreshToken,             │                              │
    │    expiresIn: 900 }          │                              │
    │◄─────────────────────────────│                              │
    │                              │                              │
    │  GET /api/shipments          │                              │
    │  Authorization: Bearer <jwt> │                              │
    │─────────────────────────────►│                              │
    │                              │  Validate JWT signature      │
    │                              │  Check "permissions" claim   │
    │                              │  → "shipments.read" ✓        │
    │  200 OK [...]                │                              │
    │◄─────────────────────────────│                              │
    │                              │                              │
    │  POST /api/auth/refresh      │                              │
    │  { refreshToken }            │                              │
    │─────────────────────────────►│                              │
    │                              │  Validate & rotate token     │
    │                              │  Revoke old, issue new pair  │
    │  { new accessToken,          │                              │
    │    new refreshToken }        │                              │
    │◄─────────────────────────────│                              │
```

### JWT Token Details

| Property | Value |
|---|---|
| Algorithm | HMAC-SHA256 |
| Access token expiry | 15 minutes |
| Refresh token expiry | 7 days |
| Issuer | `niagaone-api` |
| Audience | `niagaone-client` |
| Token claims | `sub`, `email`, `name`, `iat`, `role` (multiple), `permissions` (multiple) |

### Refresh Token Security

- Tokens are stored as **SHA256 hashes** in the database (never stored in plaintext)
- **Token rotation**: each refresh invalidates the previous token
- **Revocation**: supports single-token and bulk revocation (logout all devices)
- IP address tracking for audit trail

### RBAC — Roles & Permissions

**4 Built-in System Roles:**

| Role | Description | Permissions |
|---|---|---|
| **Admin** | Full system access | All 18 permissions |
| **Manager** | Operational management | 10 permissions (shipments, tracking, fleet, users.read) |
| **Driver** | Field operations | 3 permissions (shipments.read, tracking.read, tracking.create) |
| **Viewer** | Read-only access | 2 permissions (shipments.read, tracking.read) |

**18 Granular Permissions:**

| Resource | Permissions |
|---|---|
| Users | `users.create`, `users.read`, `users.update`, `users.delete` |
| Roles | `roles.create`, `roles.read`, `roles.update`, `roles.delete`, `roles.assign` |
| Shipments | `shipments.create`, `shipments.read`, `shipments.update`, `shipments.delete`, `shipments.assign` |
| Tracking | `tracking.create`, `tracking.read` |
| Drivers | `drivers.manage` |
| Vehicles | `vehicles.manage` |
| Warehouses | `warehouses.manage` |

Custom roles can be created and assigned any combination of permissions via the API.

---

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [MySQL 8.0](https://dev.mysql.com/downloads/mysql/) (or compatible: MariaDB 10.6+)
- Git

### 1. Clone the Repository

```bash
git clone https://github.com/mpuntodewof/NiagaOnes.git
cd NiagaOnes
```

### 2. Set Up MySQL Database

Create the database and user:

```sql
CREATE DATABASE niagaone;
CREATE USER 'log_user'@'localhost' IDENTIFIED BY 'your_secure_password';
GRANT ALL PRIVILEGES ON niagaone.* TO 'log_user'@'localhost';
FLUSH PRIVILEGES;
```

### 3. Configure Connection String

Update `API/appsettings.json` with your MySQL credentials:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "server=127.0.0.1;port=3306;database=niagaone;user=log_user;password=your_secure_password;AllowUserVariables=true;Connect Timeout=30;SslMode=Preferred"
  },
  "JwtSettings": {
    "SecretKey": "your-secret-key-at-least-32-characters-long",
    "Issuer": "niagaone-api",
    "Audience": "niagaone-client",
    "AccessTokenExpiryMinutes": 15,
    "RefreshTokenExpiryDays": 7
  }
}
```

> **Important:** Replace `SecretKey` with a strong, unique secret for production environments. Generate one with:
> ```bash
> openssl rand -base64 32
> ```

### 4. Restore Dependencies

```bash
dotnet restore
```

### 5. Run Database Migrations

Migrations run automatically on API startup. Alternatively, apply them manually:

```bash
dotnet ef database update --project Infrastructure --startup-project API
```

### 6. Run the API

```bash
cd API
dotnet run
```

The API will start at `http://localhost:5164`. Swagger UI is available at `http://localhost:5164/swagger`.

### 7. Run the Blazor Frontend

In a separate terminal:

```bash
cd BlazorApp/BlazorApp
dotnet run
```

The frontend will be available at the URL shown in the terminal output.

### 8. Verify Installation

Open Swagger UI and test the login endpoint:

```bash
curl -X POST http://localhost:5164/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email": "admin@niagaone.com", "password": "P@ssw0rd123"}'
```

You should receive a JSON response with `accessToken`, `refreshToken`, and user info.

---

## Configuration

### Application Settings (`API/appsettings.json`)

| Setting | Description | Default |
|---|---|---|
| `ConnectionStrings:DefaultConnection` | MySQL connection string | localhost:3306 |
| `JwtSettings:SecretKey` | HMAC-SHA256 signing key | — |
| `JwtSettings:Issuer` | JWT issuer claim | `niagaone-api` |
| `JwtSettings:Audience` | JWT audience claim | `niagaone-client` |
| `JwtSettings:AccessTokenExpiryMinutes` | Access token TTL | `15` |
| `JwtSettings:RefreshTokenExpiryDays` | Refresh token TTL | `7` |

### Blazor Client Settings (`BlazorApp/BlazorApp.Client/wwwroot/appsettings.json`)

| Setting | Description | Default |
|---|---|---|
| API Base URL | HttpClient base address | `http://localhost:59980/` |

> Update the Blazor client API URL to match your API's address if it differs.

### CORS

The API allows all origins by default (`AllowAnyOrigin`). For production, restrict this to your frontend's domain.

---

## Default Accounts

The database is seeded with four test accounts:

| Name | Email | Password | Role |
|---|---|---|---|
| Alice Admin | `admin@niagaone.com` | `P@ssw0rd123` | Admin |
| Marcus Manager | `manager@niagaone.com` | `P@ssw0rd123` | Manager |
| Diana Driver | `driver@niagaone.com` | `P@ssw0rd123` | Driver |
| Victor Viewer | `viewer@niagaone.com` | `P@ssw0rd123` | Viewer |

> **Warning:** Change these passwords immediately in production environments.

---

## License

This project is for educational and portfolio demonstration purposes.

---

<p align="center">
  Built with .NET 8 &middot; Clean Architecture &middot; JWT &middot; Blazor
</p>
