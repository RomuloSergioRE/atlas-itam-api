using Atlas.Itam.Domain.Entities;
using Atlas.Itam.Domain.Interfaces;
using Atlas.Itam.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Itam.Infrastructure.Repositories;

public sealed class LocationRepository : ILocationRepository
{
    private readonly AppDbContext _context;

    public LocationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Location?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Locations
            .FirstOrDefaultAsync(l => l.LocationId == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Location>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Locations
            .OrderBy(l => l.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Guid> AddAsync(Location location, CancellationToken cancellationToken = default)
    {
        await _context.Locations.AddAsync(location, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return location.LocationId;
    }

    public async Task UpdateAsync(Location location, CancellationToken cancellationToken = default)
    {
        _context.Locations.Update(location);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
