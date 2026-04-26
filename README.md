# VideoCatalog

A microservices-based video catalog application built with .NET 8, featuring an API Gateway, Identity Service, Web API, and media processor worker service.

## Architecture

The project follows Clean Architecture principles with API Gateway pattern:

```
Client → API Gateway (YARP) → Identity Service (/auth/*, /connect/*)
                         → VideoCatalog API (/api/*) [JWT Protected]
                         → Media Processor (Background Worker)
```

### Components

- **VideoCatalog.Gateway** - YARP API Gateway (Single entry point, JWT validation)
- **VideoCatalog.Identity** - Identity Service with JWT token generation
- **VideoCatalog.API** - Web API for video management (Protected by JWT)
- **VideoCatalog.Application** - Business logic, commands, and queries
- **VideoCatalog.Domain** - Domain entities and interfaces
- **VideoCatalog.Infrastructure** - Data access and repositories
- **VideoCatalog.MediaProcessor** - Background worker for media processing

## Tech Stack

- **.NET 8** (ASP.NET Core)
- **PostgreSQL 16** - Primary database
- **Redis 7** - Caching
- **RabbitMQ 3** - Message broker for event-driven communication
- **Docker & Docker Compose** - Containerization

## Prerequisites

- Docker Desktop installed and running
- .NET 8 SDK (for local development without Docker)

## Quick Start with Docker

### Build and Run All Services

```bash
docker-compose up --build
```

Or run in detached mode:

```bash
docker-compose up -d --build
```

### Access Services

After the containers are running:

- **API Gateway**: http://localhost:8080 (Main entry point)
- **Identity Service**: http://localhost:8081 (Internal, via Gateway)
- **VideoCatalog API**: http://localhost:8082 (Internal, via Gateway)
- **RabbitMQ Management UI**: http://localhost:15672
  - Username: `guest`
  - Password: `guest`
- **PostgreSQL**: `localhost:5432`
  - Database: `VideoCatalogDb` or `IdentityDb`
  - Username: `postgres`
  - Password: `postgres`
- **Redis**: `localhost:6379`

### Docker Commands

**View logs:**
```bash
docker-compose logs -f
```

**View logs for specific service:**
```bash
docker-compose logs -f api
docker-compose logs -f media-processor
```

**Stop all services:**
```bash
docker-compose down
```

**Stop and remove volumes:**
```bash
docker-compose down -v
```

**Rebuild and restart:**
```bash
docker-compose up --build -d
```

## Local Development (Without Docker)

### Prerequisites

- .NET 8 SDK
- PostgreSQL 16
- Redis 7
- RabbitMQ 3

### Configuration

Update `appsettings.json` in the respective projects with your connection strings:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=VideoCatalogDb;Username=postgres;Password=postgres",
    "Redis": "localhost:6379",
    "RabbitMQ": "amqp://guest:guest@localhost:5672"
  }
}
```

### Run the API

```bash
cd src/VideoCatalog.API
dotnet run
```

### Run the Media Processor

```bash
cd src/VideoCatalog.MediaProcessor
dotnet run
```

## Project Structure

```
BerkeTV/
├── src/
│   ├── VideoCatalog.Gateway/         # YARP API Gateway
│   │   └── Program.cs
│   ├── VideoCatalog.Identity/        # Identity Service
│   │   ├── Controllers/              # Auth and Token endpoints
│   │   ├── Data/                     # DbContext
│   │   ├── Entities/                 # User entity
│   │   └── Program.cs
│   ├── VideoCatalog.API/           # Web API entry point
│   │   ├── Controllers/            # API controllers
│   │   └── Program.cs
│   ├── VideoCatalog.Application/   # Application layer
│   │   ├── DTOs/                   # Data transfer objects
│   │   └── Features/               # CQRS handlers
│   │       ├── Videos/
│   │       │   ├── Commands/       # Write operations
│   │       │   └── Queries/        # Read operations
│   ├── VideoCatalog.Domain/        # Domain layer
│   │   ├── Entities/               # Domain entities
│   │   ├── Events/                 # Domain events
│   │   └── Interfaces/             # Repository interfaces
│   ├── VideoCatalog.Infrastructure/# Infrastructure layer
│   │   ├── Data/                   # DbContext
│   │   └── Repositories/           # Repository implementations
│   └── VideoCatalog.MediaProcessor/# Background worker
│       ├── Consumers/              # Message consumers
│       └── Worker.cs
├── docker-compose.yml
├── Dockerfile
├── Dockerfile.Identity
├── Dockerfile.Gateway
├── Dockerfile.MediaProcessor
└── VideoCatalog.slnx
```

## API Endpoints

### Authentication (via Gateway)

- `POST http://localhost:8080/auth/register` - Register a new user
- `POST http://localhost:8080/connect/token` - Get JWT token (Login)

### Videos (Protected - Requires JWT)

All video endpoints require a valid JWT token in the `Authorization` header:
```
Authorization: Bearer <your-jwt-token>
```

- `GET http://localhost:8080/api/videos` - Get all videos
- `GET http://localhost:8080/api/videos/{id}` - Get video by ID
- `POST http://localhost:8080/api/videos` - Create a new video

## Docker Services

The Docker Compose setup includes:

1. **postgres** - PostgreSQL database with health checks
2. **redis** - Redis cache with health checks
3. **rabbitmq** - RabbitMQ message broker with management UI
4. **identity** - Identity Service for JWT token generation (depends on postgres)
5. **api** - .NET Web API (depends on postgres, redis, rabbitmq)
6. **gateway** - YARP API Gateway (depends on identity, api)
7. **media-processor** - Background worker for media processing (depends on rabbitmq)

All services are connected via the `videocatalog-network` bridge network.

## Authentication Flow

### 1. Register a User

```bash
curl -X POST http://localhost:8080/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "username": "testuser",
    "email": "test@example.com",
    "password": "SecurePass123!"
  }'
```

### 2. Login and Get JWT Token

```bash
curl -X POST http://localhost:8080/connect/token \
  -H "Content-Type: application/json" \
  -d '{
    "grantType": "password",
    "username": "testuser",
    "password": "SecurePass123!"
  }'
```

Response:
```json
{
  "access_token": "eyJhbGciOiJIUzI1NiIs...",
  "token_type": "Bearer",
  "expires_in": 3600
}
```

### 3. Access Protected API

```bash
curl -X GET http://localhost:8080/api/videos \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIs..."
```

## Swagger UI

Swagger UI is available through the Gateway for API documentation and testing:

### Access Swagger (No Authentication Required)

Simply open in your browser:
```
http://localhost:8080/swagger/index.html
```

### Test Protected Endpoints

To test API endpoints that require authentication:

1. **Get a JWT token**:
   ```powershell
   $token = (Invoke-RestMethod -Uri "http://localhost:8080/connect/token" -Method POST -ContentType "application/json" -Body '{"grantType":"password","username":"testuser","password":"SecurePass123!"}').access_token
   ```

2. **Authorize in Swagger**:
   - Open Swagger UI
   - Click the **"Authorize"** button (🔒 icon)
   - Enter: `Bearer <your-jwt-token>`
   - Click **"Authorize"**
   - Now you can test protected endpoints!

Alternatively, access Swagger directly on the API service (development only):
- http://localhost:8082/swagger/index.html

## Environment Variables

### Gateway Service

- `ASPNETCORE_ENVIRONMENT` - Environment (Development/Production)
- `ASPNETCORE_URLS` - URLs to listen on

### Identity Service

- `ASPNETCORE_ENVIRONMENT` - Environment (Development/Production)
- `ASPNETCORE_URLS` - URLs to listen on
- `ConnectionStrings__DefaultConnection` - PostgreSQL connection string
- `Jwt__Issuer` - JWT token issuer
- `Jwt__Audience` - JWT token audience
- `Jwt__SecretKey` - JWT signing key

### API Service

- `ASPNETCORE_ENVIRONMENT` - Environment (Development/Production)
- `ASPNETCORE_URLS` - URLs to listen on
- `ConnectionStrings__DefaultConnection` - PostgreSQL connection string
- `ConnectionStrings__Redis` - Redis connection string
- `ConnectionStrings__RabbitMQ` - RabbitMQ connection string

### Media Processor Service

- `DOTNET_ENVIRONMENT` - Environment (Development/Production)
- `ConnectionStrings__RabbitMQ` - RabbitMQ connection string

## Persistent Data

Docker volumes are used to persist data:

- `postgres_data` - PostgreSQL database files
- `redis_data` - Redis data
- `rabbitmq_data` - RabbitMQ data

## Security Architecture

### How It Works

1. **API Gateway (YARP)**: The single entry point for all client requests
   - Validates JWT tokens before forwarding to backend services
   - Routes `/auth/*` and `/connect/*` to Identity Service (no auth required)
   - Routes `/api/*` to VideoCatalog API (JWT required)

2. **Identity Service**: Handles authentication and token generation
   - User registration with password hashing (BCrypt)
   - JWT token generation with configurable signing key
   - Token endpoint: `/connect/token`

3. **VideoCatalog API**: Protected microservice
   - Requires valid JWT token for all endpoints
   - Validates tokens using the same signing key as Identity Service
   - Trusts the Gateway's authentication stamp

### Security Best Practices

- All API endpoints are protected by default
- Passwords are hashed using BCrypt
- JWT tokens expire after 1 hour
- Signing key should be moved to environment variables in production
- Internal services are not directly exposed to the public internet
- HTTPS should be enabled in production

## Troubleshooting

### Port Already in Use

If ports 5432, 6379, 5672, 15672, 8080, 8081, or 8082 are already in use:

1. Stop the conflicting service, or
2. Modify the port mappings in `docker-compose.yml`

### Database Connection Issues

Ensure PostgreSQL health check passes before the API starts. The API will wait for PostgreSQL to be ready.

### View Container Status

```bash
docker-compose ps
```

### Clean Restart

```bash
docker-compose down -v
docker-compose up --build
```

## License

This project is private.
