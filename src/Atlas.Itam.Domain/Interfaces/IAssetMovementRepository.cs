using Atlas.Itam.Domain.Entities;

namespace Atlas.Itam.Domain.Interfaces;

public interface IAssetMovementRepository
{
    Task<IReadOnlyList<AssetMovement>> GetByAssetIdAsync(Guid assetId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AssetMovement>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Guid> AddAsync(AssetMovement movement, CancellationToken cancellationToken = default);
}
