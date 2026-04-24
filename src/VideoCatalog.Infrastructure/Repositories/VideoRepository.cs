using Microsoft.EntityFrameworkCore;
using VideoCatalog.Domain.Entities;
using VideoCatalog.Domain.Interfaces;
using VideoCatalog.Infrastructure.Data;

namespace VideoCatalog.Infrastructure.Repositories;

public class VideoRepository : IVideoRepository
{
    private readonly VideoCatalogDbContext _context;

    public VideoRepository(VideoCatalogDbContext context) => _context = context;

    public async Task<Video?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _context.Videos.FindAsync([id], ct);

    public async Task<IEnumerable<Video>> GetAllAsync(CancellationToken ct = default) =>
        await _context.Videos.ToListAsync(ct);

    public async Task AddAsync(Video video, CancellationToken ct = default)
    {
        await _context.Videos.AddAsync(video, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Video video, CancellationToken ct = default)
    {
        _context.Videos.Update(video);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var video = await GetByIdAsync(id, ct);
        if (video != null)
        {
            _context.Videos.Remove(video);
            await _context.SaveChangesAsync(ct);
        }
    }

    public async Task SaveChangesAsync(CancellationToken ct = default) =>
        await _context.SaveChangesAsync(ct);
}
