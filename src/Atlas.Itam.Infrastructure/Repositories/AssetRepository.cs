using Atlas.Itam.Domain.Entities;
using Atlas.Itam.Domain.Enums;
using Atlas.Itam.Domain.Interfaces;
using Atlas.Itam.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Itam.Infrastructure.Repositories;

public sealed class AssetRepository : IAssetRepository
{
    private readonly AppDbContext _context;

    public AssetRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Asset?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Assets
            .Include(a => a.Category)
            .Include(a => a.Location)
            .Include(a => a.CurrentUser)
            .FirstOrDefaultAsync(a => a.AssetId == id, cancellationToken);
    }

    public async Task<Asset?> GetByPatrimonyNumberAsync(string patrimonyNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Assets
            .Include(a => a.Category)
            .Include(a => a.Location)
            .Include(a => a.CurrentUser)
            .FirstOrDefaultAsync(a => a.PatrimonyNumber == patrimonyNumber, cancellationToken);
    }

    public async Task<Asset?> GetBySerialNumberAsync(string serialNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Assets
            .Include(a => a.Category)
            .Include(a => a.Location)
            .Include(a => a.CurrentUser)
            .FirstOrDefaultAsync(a => a.SerialNumber == serialNumber, cancellationToken);
    }

    public async Task<IReadOnlyList<Asset>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Assets
            .Include(a => a.Category)
            .Include(a => a.Location)
            .Include(a => a.CurrentUser)
            .Where(a => !a.IsDeleted)
            .OrderBy(a => a.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Asset>> GetByStatusAsync(AssetStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.Assets
            .Include(a => a.Category)
            .Include(a => a.Location)
            .Where(a => a.Status == status && !a.IsDeleted)
            .OrderBy(a => a.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Asset>> GetByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        return await _context.Assets
            .Include(a => a.Category)
            .Include(a => a.Location)
            .Where(a => a.CategoryId == categoryId && !a.IsDeleted)
            .OrderBy(a => a.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Asset>> GetByLocationAsync(Guid locationId, CancellationToken cancellationToken = default)
    {
        return await _context.Assets
            .Include(a => a.Category)
            .Include(a => a.Location)
            .Where(a => a.LocationId == locationId && !a.IsDeleted)
            .OrderBy(a => a.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Asset>> GetByCurrentUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Assets
            .Include(a => a.Category)
            .Include(a => a.Location)
            .Where(a => a.CurrentUserId == userId && !a.IsDeleted)
            .OrderBy(a => a.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Asset>> SearchAsync(string term, CancellationToken cancellationToken = default)
    {
        var lowerTerm = term.ToLowerInvariant();

        return await _context.Assets
            .Include(a => a.Category)
            .Include(a => a.Location)
            .Include(a => a.CurrentUser)
            .Where(a => !a.IsDeleted && (
                a.Name.ToLower().Contains(lowerTerm) ||
                a.PatrimonyNumber.ToLower().Contains(lowerTerm) ||
                a.SerialNumber.ToLower().Contains(lowerTerm)))
            .OrderBy(a => a.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByPatrimonyNumberAsync(string patrimonyNumber, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        return await _context.Assets
            .AnyAsync(a => a.PatrimonyNumber == patrimonyNumber && (!excludeId.HasValue || a.AssetId != excludeId.Value), cancellationToken);
    }

    public async Task<bool> ExistsBySerialNumberAsync(string serialNumber, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        return await _context.Assets
            .AnyAsync(a => a.SerialNumber == serialNumber && (!excludeId.HasValue || a.AssetId != excludeId.Value), cancellationToken);
    }

    public async Task<Guid> AddAsync(Asset asset, CancellationToken cancellationToken = default)
    {
        await _context.Assets.AddAsync(asset, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return asset.AssetId;
    }

    public async Task UpdateAsync(Asset asset, CancellationToken cancellationToken = default)
    {
        _context.Assets.Update(asset);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Asset asset, CancellationToken cancellationToken = default)
    {
        _context.Assets.Remove(asset);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
