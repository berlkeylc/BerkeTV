namespace VideoCatalog.Domain.Events;

public record VideoUploadedIntegrationEvent(
    Guid Id,
    string Title,
    string Description,
    string Url,
    TimeSpan Duration,
    string? ThumbnailUrl,
    DateTime UploadedAt);
