using Atlas.Itam.Domain.Entities;
using Atlas.Itam.Domain.Interfaces;
using Atlas.Itam.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Itam.Infrastructure.Repositories;

public sealed class AssetMovementRepository : IAssetMovementRepository
{
    private readonly AppDbContext _context;

    public AssetMovementRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<AssetMovement>> GetByAssetIdAsync(Guid assetId, CancellationToken cancellationToken = default)
    {
        return await _context.AssetMovements
            .Include(m => m.Asset)
            .Include(m => m.FromUser)
            .Include(m => m.ToUser)
            .Include(m => m.Responsible)
            .Where(m => m.AssetId == assetId)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AssetMovement>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.AssetMovements
            .Include(m => m.Asset)
            .Include(m => m.FromUser)
            .Include(m => m.ToUser)
            .Include(m => m.Responsible)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Guid> AddAsync(AssetMovement movement, CancellationToken cancellationToken = default)
    {
        await _context.AssetMovements.AddAsync(movement, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return movement.MovementId;
    }
}
