using Atlas.Itam.Domain.Entities;
using Atlas.Itam.Domain.Enums;

namespace Atlas.Itam.Domain.Interfaces;

public interface IAssetRepository
{
    Task<Asset?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Asset?> GetByPatrimonyNumberAsync(string patrimonyNumber, CancellationToken cancellationToken = default);
    Task<Asset?> GetBySerialNumberAsync(string serialNumber, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Asset>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Asset> Items, int TotalCount)> GetAllAsync(string? search, AssetStatus? status, Guid? categoryId, Guid? locationId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Asset>> GetByStatusAsync(AssetStatus status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Asset>> GetByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Asset>> GetByLocationAsync(Guid locationId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Asset>> GetByCurrentUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Asset>> SearchAsync(string term, CancellationToken cancellationToken = default);
    Task<bool> ExistsByPatrimonyNumberAsync(string patrimonyNumber, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task<bool> ExistsBySerialNumberAsync(string serialNumber, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task<Guid> AddAsync(Asset asset, CancellationToken cancellationToken = default);
    Task UpdateAsync(Asset asset, CancellationToken cancellationToken = default);
    Task DeleteAsync(Asset asset, CancellationToken cancellationToken = default);
}
