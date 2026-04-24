using MediatR;
using VideoCatalog.Application.DTOs;
using VideoCatalog.Domain.Entities;
using VideoCatalog.Domain.Interfaces;

namespace VideoCatalog.Application.Features.Videos.Commands;

public class CreateVideoCommandHandler : IRequestHandler<CreateVideoCommand, VideoDto>
{
    private readonly IVideoRepository _repository;

    public CreateVideoCommandHandler(IVideoRepository repository)
    {
        _repository = repository;
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
