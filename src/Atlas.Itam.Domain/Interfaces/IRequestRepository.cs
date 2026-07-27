using Atlas.Itam.Domain.Entities;
using Atlas.Itam.Domain.Enums;

namespace Atlas.Itam.Domain.Interfaces;

public interface IRequestRepository
{
    Task<Request?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Request>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Request>> GetByStatusAsync(RequestStatus status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Request>> GetByRequestedByAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Request>> GetPendingByDepartmentAsync(Guid departmentId, CancellationToken cancellationToken = default);
    Task<int> CountPendingByUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> HasActiveRequestForAssetAsync(Guid assetId, CancellationToken cancellationToken = default);
    Task<Guid> AddAsync(Request request, CancellationToken cancellationToken = default);
    Task UpdateAsync(Request request, CancellationToken cancellationToken = default);
}
