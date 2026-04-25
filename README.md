# VideoCatalog

A microservices-based video catalog application built with .NET 8, featuring a Web API and media processor worker service.

## Architecture

The project follows Clean Architecture principles with the following layers:

- **VideoCatalog.API** - Web API for video management
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

- **Web API**: http://localhost:8080
- **RabbitMQ Management UI**: http://localhost:15672
  - Username: `guest`
  - Password: `guest`
- **PostgreSQL**: `localhost:5432`
  - Database: `VideoCatalogDb`
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
├── Dockerfile.MediaProcessor
└── VideoCatalog.slnx
```

## API Endpoints

### Videos

- `GET /api/videos` - Get all videos
- `GET /api/videos/{id}` - Get video by ID
- `POST /api/videos` - Create a new video

## Docker Services

The Docker Compose setup includes:

1. **postgres** - PostgreSQL database with health checks
2. **redis** - Redis cache with health checks
3. **rabbitmq** - RabbitMQ message broker with management UI
4. **api** - .NET Web API (depends on postgres, redis, rabbitmq)
5. **media-processor** - Background worker for media processing (depends on rabbitmq)

All services are connected via the `videocatalog-network` bridge network.

## Environment Variables

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

## Troubleshooting

### Port Already in Use

If ports 5432, 6379, 5672, 15672, or 8080 are already in use:

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
