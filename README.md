# AgriMart


AgriMartis a robust ASP.NET Core Web API designed for managing an agricultural marketplace. It provides a layered architecture with secure endpoints, data access, authentication, and middleware to support a scalable and maintainable backend service.

## Features

- RESTful API endpoints for managing orders, products, user profiles, categories, and more.
- JWT Bearer authentication with role-based authorization (Manager, Admin, Customer).
- Repository pattern for data access using ADO.NET and SQL Server.
- Custom middleware for request timing and logging.
- Swagger integration for interactive API documentation and testing.
- CORS configuration for secure cross-origin requests.
- User registration, OTP verification, and JWT token generation for authentication.
- Dependency injection for services and repositories.

## Architecture

- **Controllers:** Handle HTTP requests and responses.
- **Services:** Contain business logic and authentication services.
- **Repositories:** Abstract data access logic using raw SQL queries.
- **Models:** Define data entities corresponding to database tables.
- **Middleware:** Custom middleware for logging request durations.
- **Security:** JWT authentication and role-based authorization policies.

## Getting Started

### Prerequisites

- .NET 6.0 SDK or later
- SQL Server database
- Visual Studio 2022 or VS Code

### Configuration

1. Update the connection string in `appsettings.json` or `appsettings.Development.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Your SQL Server connection string here"
}
```

2. Configure JWT settings in `appsettings.json`:

```json
"Jwt": {
  "Key": "YourSecretKeyHere",
  "Issuer": "YourIssuer",
  "Audience": "YourAudience"
}
```

### Running the API

1. Restore dependencies:

```bash
dotnet restore
```

2. Build the project:

```bash
dotnet build
```

3. Run the API:

```bash
dotnet run --project src/AgriMartAPI.csproj
```

4. Access Swagger UI for API documentation and testing at:

```
https://localhost:{port}/swagger
```

## Testing

Currently, minimal automated tests exist. It is recommended to add unit and integration tests for critical components.

## Contributing

Contributions are welcome! Please fork the repository and submit pull requests.

