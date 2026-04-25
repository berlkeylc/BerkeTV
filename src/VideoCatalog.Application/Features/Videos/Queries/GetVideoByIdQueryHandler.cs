using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using VideoCatalog.Application.DTOs;
using VideoCatalog.Domain.Interfaces;

namespace VideoCatalog.Application.Features.Videos.Queries;

public class GetVideoByIdQueryHandler : IRequestHandler<GetVideoByIdQuery, VideoDto?>
{
    private readonly IVideoRepository _repository;
    private readonly IDistributedCache _cache;
    private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(10);

    public GetVideoByIdQueryHandler(IVideoRepository repository, IDistributedCache cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<VideoDto?> Handle(GetVideoByIdQuery request, CancellationToken cancellationToken)
    {
        // Step 1: Try to get from cache
        var cacheKey = $"video:{request.Id}";
        var cachedVideo = await _cache.GetStringAsync(cacheKey, cancellationToken);

        if (!string.IsNullOrEmpty(cachedVideo))
        {
            // Cache hit - deserialize and return
            return JsonSerializer.Deserialize<VideoDto>(cachedVideo);
        }

        // Step 2: Cache miss - fetch from database
        var video = await _repository.GetByIdAsync(request.Id, cancellationToken);
        
        if (video == null)
        {
            return null;
        }

        var videoDto = new VideoDto(
            video.Id,
            video.Title,
            video.Description,
            video.Url,
            video.ThumbnailUrl,
            video.Duration,
            video.CreatedAt,
            video.UpdatedAt
        );

        // Step 3: Save to cache with expiration
        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = _cacheExpiration
        };

        await _cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(videoDto),
            cacheOptions,
            cancellationToken);

        // Step 4: Return the data
        return videoDto;
    }
}
