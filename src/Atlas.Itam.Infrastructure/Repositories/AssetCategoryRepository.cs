using Atlas.Itam.Domain.Entities;
using Atlas.Itam.Domain.Interfaces;
using Atlas.Itam.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Itam.Infrastructure.Repositories;

public sealed class AssetCategoryRepository : IAssetCategoryRepository
{
    private readonly AppDbContext _context;

    public AssetCategoryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<AssetCategory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.AssetCategories
            .FirstOrDefaultAsync(c => c.CategoryId == id, cancellationToken);
    }

    public async Task<IReadOnlyList<AssetCategory>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.AssetCategories
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AssetCategory>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.AssetCategories
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> HasAssetsAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        return await _context.Assets
            .AnyAsync(a => a.CategoryId == categoryId && !a.IsDeleted, cancellationToken);
    }

    public async Task<Guid> AddAsync(AssetCategory category, CancellationToken cancellationToken = default)
    {
        await _context.AssetCategories.AddAsync(category, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return category.CategoryId;
    }

    public async Task UpdateAsync(AssetCategory category, CancellationToken cancellationToken = default)
    {
        _context.AssetCategories.Update(category);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(AssetCategory category, CancellationToken cancellationToken = default)
    {
        _context.AssetCategories.Remove(category);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
