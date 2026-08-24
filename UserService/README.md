# UserService

## Overview
The `UserService` project is a .NET application designed to manage user authentication, authorization, and registration. It leverages MediatR for handling commands and queries, and includes support for JWT-based authentication.

## Features
- User registration and login functionality.
- Role-based authorization using custom policies.
- JWT token generation and validation.
- Integration with ASP.NET Identity for user management.
- Command and Query handling using MediatR.

## Project Structure
- **Commands/**: Contains command classes like `LoginCommand` and `RegisterCommand`.
- **Consts/**: Holds constant values such as `CustomClaims`.
- **Entities/**: Defines the `User` entity.
- **Enum/**: Includes enumerations like `LoginStatus`.
- **Interfaces/**: Contains interfaces for abstraction, such as `IApplicationUser`, `ITokenProvider`, and `IPermissionProvider`.
- **Migrations/**: Entity Framework migrations for database schema changes.
- **Options/**: Configuration options like `JwtOption`.
- **Persistence/**: Database context (`UserDbContext`).
- **Providers/**: Implementation of providers for identity, roles, and tokens.
- **Responses/**: Response models like `LoginResponse` and `RegistrationResponse`.

## Getting Started

### Prerequisites
- .NET 8.0 SDK
- SQL Server or any compatible database

### Setup
1. Clone the repository.
2. Navigate to the project directory.
3. Restore dependencies:
   ```bash
   dotnet restore
   ```
4. Apply migrations to the database:
   ```bash
   dotnet ef database update
   ```
5. Run the application:
   ```bash
   dotnet run
   ```

### Database Update
To apply migrations to the database, use the following command:


```Package Consol Manager
update-database -Context UserDbContext
```

```bash
dotnet ef database update -Context UserDbContext
```


This ensures that the database schema is up-to-date with the latest migrations.

## Usage
- Configure JWT options in `appsettings.json` under the `Jwt` section.
- Use the `AddUserServices` method to set up UserService In Your Project.

## Contributing
Contributions are welcome! Please fork the repository and submit a pull request.

## License
This project is licensed under the MIT License.