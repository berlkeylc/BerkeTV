using MassTransit;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using VideoCatalog.Application.DTOs;
using VideoCatalog.Domain.Entities;
using VideoCatalog.Domain.Events;
using VideoCatalog.Domain.Interfaces;

namespace VideoCatalog.Application.Features.Videos.Commands;

public class CreateVideoCommandHandler : IRequestHandler<CreateVideoCommand, VideoDto>
{
    private readonly IVideoRepository _repository;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IDistributedCache _cache;

    public CreateVideoCommandHandler(IVideoRepository repository, IPublishEndpoint publishEndpoint, IDistributedCache cache)
    {
        _repository = repository;
        _publishEndpoint = publishEndpoint;
        _cache = cache;
    }

    public async Task<VideoDto> Handle(CreateVideoCommand request, CancellationToken cancellationToken)
    {
        // Create domain entity
        var video = new Video(
            request.Title,
            request.Description,
            request.Url,
            request.Duration
        );

        // Set optional thumbnail if provided
        if (!string.IsNullOrEmpty(request.ThumbnailUrl))
        {
            video.SetThumbnailUrl(request.ThumbnailUrl);
        }

        // Save to database via repository
        await _repository.AddAsync(video, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        // Invalidate cache for video lists (if you implement GetAllVideos caching)
        await _cache.RemoveAsync("videos:all", cancellationToken);

        // Publish integration event
        var integrationEvent = new VideoUploadedIntegrationEvent(
            video.Id,
            video.Title,
            video.Description,
            video.Url,
            video.Duration,
            video.ThumbnailUrl,
            video.CreatedAt
        );
        await _publishEndpoint.Publish(integrationEvent, cancellationToken);

        // Return DTO
        return new VideoDto(
            video.Id,
            video.Title,
            video.Description,
            video.Url,
            video.ThumbnailUrl,
            video.Duration,
            video.CreatedAt,
            video.UpdatedAt
        );
    }
}
