using MassTransit;
using VideoCatalog.MediaProcessor;
using VideoCatalog.MediaProcessor.Consumers;

var builder = Host.CreateApplicationBuilder(args);

// MassTransit with RabbitMQ - Consumer
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<VideoUploadedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("RabbitMQ") ?? "amqp://guest:guest@localhost:5672", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ReceiveEndpoint("video-uploaded-queue", e =>
        {
            e.ConfigureConsumer<VideoUploadedConsumer>(context);
        });
    });
});

var host = builder.Build();
host.Run();
