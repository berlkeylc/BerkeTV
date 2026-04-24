using Microsoft.EntityFrameworkCore;
using VideoCatalog.Domain.Entities;

namespace VideoCatalog.Infrastructure.Data;

public class VideoCatalogDbContext : DbContext
{
    public VideoCatalogDbContext(DbContextOptions<VideoCatalogDbContext> options) : base(options) { }

    public DbSet<Video> Videos => Set<Video>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Video>(entity =>
        {
            entity.HasKey(v => v.Id);
            entity.Property(v => v.Title).IsRequired().HasMaxLength(200);
            entity.Property(v => v.Description).HasMaxLength(2000);
            entity.Property(v => v.Url).IsRequired().HasMaxLength(500);
            entity.Property(v => v.ThumbnailUrl).HasMaxLength(500);
        });
    }
}
