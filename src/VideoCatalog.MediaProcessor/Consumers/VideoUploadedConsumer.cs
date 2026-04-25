using MassTransit;
using Microsoft.Extensions.Logging;
using VideoCatalog.Domain.Events;

namespace VideoCatalog.MediaProcessor.Consumers;

public class VideoUploadedConsumer : IConsumer<VideoUploadedIntegrationEvent>
{
    private readonly ILogger<VideoUploadedConsumer> _logger;

    public VideoUploadedConsumer(ILogger<VideoUploadedConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<VideoUploadedIntegrationEvent> context)
    {
        var message = context.Message;
        
        _logger.LogInformation("Starting Transcoding for Video ID: {Id}", message.Id);
        
        // TODO: Add transcoding logic here
        // For example:
        // - Download video from URL
        // - Process/transcode to different formats
        // - Upload processed files to storage
        // - Update database with processed status
        
        return Task.CompletedTask;
    }
}
