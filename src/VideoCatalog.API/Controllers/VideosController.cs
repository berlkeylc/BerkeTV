using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VideoCatalog.Application.Features.Videos.Commands;
using VideoCatalog.Application.Features.Videos.Queries;

namespace VideoCatalog.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class VideosController : ControllerBase
{
    private readonly IMediator _mediator;

    public VideosController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var videos = await _mediator.Send(new GetAllVideosQuery(), ct);
        return Ok(videos);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var video = await _mediator.Send(new GetVideoByIdQuery(id), ct);
        
        if (video == null)
        {
            return NotFound();
        }

        return Ok(video);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateVideoCommand command, CancellationToken ct)
    {
        var video = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = video.Id }, video);
    }
}
