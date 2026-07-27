using Atlas.Itam.Domain.Entities;

namespace Atlas.Itam.Domain.Interfaces;

public interface IAssetCategoryRepository
{
    Task<AssetCategory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AssetCategory>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AssetCategory>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<bool> HasAssetsAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task<Guid> AddAsync(AssetCategory category, CancellationToken cancellationToken = default);
    Task UpdateAsync(AssetCategory category, CancellationToken cancellationToken = default);
    Task DeleteAsync(AssetCategory category, CancellationToken cancellationToken = default);
}
