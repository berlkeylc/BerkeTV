using MediatR;
using VideoCatalog.Application.DTOs;

namespace VideoCatalog.Application.Features.Videos.Commands;

public record CreateVideoCommand(
    string Title,
    string Description,
    string Url,
    TimeSpan Duration,
    string? ThumbnailUrl = null) : IRequest<VideoDto>;
