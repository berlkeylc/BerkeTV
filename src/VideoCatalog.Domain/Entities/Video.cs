namespace VideoCatalog.Domain.Entities;

public class Video
{
    public Guid Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string Url { get; private set; } = string.Empty;
    public string? ThumbnailUrl { get; private set; }
    public TimeSpan Duration { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private Video() { } // EF Core constructor

    public Video(string title, string description, string url, TimeSpan duration)
    {
        Id = Guid.NewGuid();
        Title = title;
        Description = description;
        Url = url;
        Duration = duration;
        CreatedAt = DateTime.UtcNow;
    }

    public void Update(string title, string description, TimeSpan duration)
    {
        Title = title;
        Description = description;
        Duration = duration;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetThumbnailUrl(string? thumbnailUrl)
    {
        ThumbnailUrl = thumbnailUrl;
    }
}
