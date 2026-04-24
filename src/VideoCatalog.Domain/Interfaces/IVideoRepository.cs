using VideoCatalog.Domain.Entities;

namespace VideoCatalog.Domain.Interfaces;

public interface IVideoRepository
{
    Task<Video?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<Video>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(Video video, CancellationToken ct = default);
    Task UpdateAsync(Video video, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
