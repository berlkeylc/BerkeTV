# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["src/VideoCatalog.API/VideoCatalog.API.csproj", "src/VideoCatalog.API/"]
COPY ["src/VideoCatalog.Application/VideoCatalog.Application.csproj", "src/VideoCatalog.Application/"]
COPY ["src/VideoCatalog.Domain/VideoCatalog.Domain.csproj", "src/VideoCatalog.Domain/"]
COPY ["src/VideoCatalog.Infrastructure/VideoCatalog.Infrastructure.csproj", "src/VideoCatalog.Infrastructure/"]
RUN dotnet restore "src/VideoCatalog.API/VideoCatalog.API.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/src/VideoCatalog.API"
RUN dotnet build "VideoCatalog.API.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "VideoCatalog.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VideoCatalog.API.dll"]
