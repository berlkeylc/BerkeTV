using MediatR;
using VideoCatalog.Application.DTOs;
using VideoCatalog.Domain.Interfaces;

namespace VideoCatalog.Application.Features.Videos.Queries;

public record GetAllVideosQuery : IRequest<IEnumerable<VideoDto>>;

public class GetAllVideosQueryHandler : IRequestHandler<GetAllVideosQuery, IEnumerable<VideoDto>>
{
    private readonly IVideoRepository _repository;

    public GetAllVideosQueryHandler(IVideoRepository repository) => _repository = repository;

    public async Task<IEnumerable<VideoDto>> Handle(GetAllVideosQuery request, CancellationToken ct)
    {
        var videos = await _repository.GetAllAsync(ct);
        return videos.Select(v => new VideoDto(v.Id, v.Title, v.Description, v.Url, v.ThumbnailUrl, v.Duration, v.CreatedAt, v.UpdatedAt));
    }
}
