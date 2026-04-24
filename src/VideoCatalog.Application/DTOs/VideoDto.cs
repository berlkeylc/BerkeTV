namespace VideoCatalog.Application.DTOs;

public record VideoDto(
    Guid Id,
    string Title,
    string Description,
    string Url,
    string? ThumbnailUrl,
    TimeSpan Duration,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
