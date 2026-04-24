using MediatR;
using VideoCatalog.Application.DTOs;
using VideoCatalog.Domain.Interfaces;

namespace VideoCatalog.Application.Features.Videos.Queries;

public class GetVideoByIdQueryHandler : IRequestHandler<GetVideoByIdQuery, VideoDto?>
{
    private readonly IVideoRepository _repository;

    public GetVideoByIdQueryHandler(IVideoRepository repository)
    {
        _repository = repository;
    }

    public async Task<VideoDto?> Handle(GetVideoByIdQuery request, CancellationToken cancellationToken)
    {
        var video = await _repository.GetByIdAsync(request.Id, cancellationToken);
        
        if (video == null)
        {
            return null;
        }

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
