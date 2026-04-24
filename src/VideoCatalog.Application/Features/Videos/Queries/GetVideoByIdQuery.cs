using MediatR;
using VideoCatalog.Application.DTOs;

namespace VideoCatalog.Application.Features.Videos.Queries;

public record GetVideoByIdQuery(Guid Id) : IRequest<VideoDto?>;
