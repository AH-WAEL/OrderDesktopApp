# Desktop API & Order Management Platform

A .NET 8 multi-project solution for a Windows desktop application with authentication, product management, and real-time order updates. This repository is structured as a showcase project for recruiters and for demonstrating full-stack .NET architecture with UI, API, data access, and messaging components.

## Overview

This solution combines a WinForms desktop client with a JWT-authenticated API layer and supporting services for product management and live updates. The project demonstrates a practical business workflow: a user registers and signs in, receives a JWT token, accesses protected product endpoints, and consumes real-time updates from a messaging backend.

## Architecture

The solution is organized into the following parts:

- desktopAPI: Windows desktop application built with WinForms
- AuthApi: ASP.NET Core API for user registration, login, token refresh, and logout
- ProductApi: ASP.NET Core API for product CRUD operations with JWT validation and Redis caching
- Receiver: SignalR/RabbitMQ receiver service for order feed updates
- first: legacy or experimental service kept for compatibility during development

## High-Level Workflow

1. User opens the Windows desktop app.
2. User registers or signs in through the auth API.
3. The app receives a JWT access token and refresh token.
4. Protected product requests are sent through the desktop app using a delegated HTTP handler.
5. Product data is retrieved from the ProductApi.
6. Real-time update messages can be received via the RabbitMQ/SignalR receiver.

## Features

- User registration and login
- JWT access and refresh tokens
- Role-based authorization support
- Product CRUD operations
- WinForms desktop interface
- Protected API endpoints
- Redis caching for product API reads
- SignalR hub for streaming updates
- RabbitMQ event receiver for background message handling
- Structured logging for actions and errors

## Tech Stack

- C# / .NET 8
- Windows Forms (WinForms)
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- JWT authentication
- Redis
- RabbitMQ
- SignalR
- Serilog

## Repository Structure

```text
desktopAPI/
├── .github/
│   └── workflows/
├── AuthApi/
│   ├── Controllers/
│   ├── Data/
│   ├── Entities/
│   ├── Models/
│   ├── services/
│   ├── appsettings.json
│   ├── AuthApi.csproj
│   └── Program.cs
├── desktopAPI/
│   ├── Models/
│   ├── Services/
│   ├── Logs/
│   ├── Program.cs
│   ├── Login.cs
│   ├── RegisterForm.cs
│   ├── ProductsForm.cs
│   ├── CreateProduct.cs
│   └── desktopAPI.csproj
├── ProductApi/
│   ├── Controllers/
│   ├── Data/
│   ├── Services/
│   ├── appsettings.json
│   ├── Program.cs
│   └── ProductApi.csproj
├── Receiver/
│   ├── Hubs/
│   ├── Program.cs
│   ├── receive.cs
│   └── Receiver.csproj
├── .gitignore
├── desktopAPI.sln
├── LICENSE
├── README.md
└── ...
```

## Prerequisites

Before running the project locally, install:

- .NET 8 SDK
- SQL Server or SQL Server Express
- Redis (optional for product API caching)
- RabbitMQ (optional for live order feed)
- Visual Studio 2022 with .NET desktop development workload

## Configuration

The repository intentionally uses placeholder values for secrets and local connection strings instead of real credentials. Before running the app, update the configuration files in:

- AuthApi/appsettings.json
- ProductApi/appsettings.json

Set the following values for your local environment:

- JWT secret key
- JWT issuer
- JWT audience
- SQL connection strings
- Redis connection string (if enabled)

Example:

```json
"JwtSettings": {
  "SecretKey": "YourVeryStrongSecretKeyHere",
  "Issuer": "YourAuthApi",
  "Audience": "DesktopClient"
},
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=YourDb;User Id=your_user;Password=your_password;TrustServerCertificate=True;Encrypt=False;",
  "Redis": "localhost:6379"
}
```

> Do not commit production secrets or real database credentials to a public GitHub repository.

## Run the project locally

### 1. Restore NuGet packages

```bash
dotnet restore desktopAPI.sln
```

### 2. Build the solution

```bash
dotnet build desktopAPI.sln
```

### 3. Start the auth API

From the repository root:

```bash
dotnet run --project AuthApi/AuthApi.csproj
```

### 4. Start the product API

```bash
dotnet run --project ProductApi/ProductApi.csproj
```

### 5. Start the message receiver

```bash
dotnet run --project Receiver/Receiver.csproj
```

### 6. Launch the desktop client

Open the solution in Visual Studio and run the desktopAPI project, or use:

```bash
dotnet run --project desktopAPI/desktopAPI.csproj
```

## Login and Usage

- Register a new user from the desktop app or through the auth API.
- Sign in with the created account.
- Use the JWT session to access protected product routes.
- View product data and create, update, or delete items depending on your workflow.
- Observe messaging updates through the SignalR receiver when the event pipeline is active.

## Security Notes

This project was prepared for public GitHub sharing and intentionally removes embedded credentials and environment-specific values. It is still a learning/demo project and should not be treated as production-grade security infrastructure without additional hardening.

Recommended production improvements:

- Move secrets to user secrets or environment variables
- Add HTTPS certificate configuration
- Add centralized configuration management
- Add request validation and rate limiting
- Introduce automated integration tests
- Add a proper CI/CD pipeline for deployment

## Roadmap / Potential Improvements

- Clean up legacy code and unify naming across all projects
- Refactor WinForms logic into view models and service abstractions
- Add unit and integration tests
- Add database migrations documentation
- Improve error handling throughout the UI and APIs
- Add admin dashboard features
- Add API versioning and health checks
- Introduce Docker support and environment orchestration

## License

This project is licensed under the MIT License. See [LICENSE](LICENSE) for details.

## Author Note

This repository is intended to showcase .NET application architecture, API design, authentication, and message-based integration in a way that is recruiter-friendly and easy to understand.
