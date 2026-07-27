using Atlas.Itam.Domain.Entities;
using Atlas.Itam.Domain.Enums;
using Atlas.Itam.Domain.Interfaces;
using Atlas.Itam.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Itam.Infrastructure.Repositories;

public sealed class RequestRepository : IRequestRepository
{
    private readonly AppDbContext _context;

    public RequestRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Request?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Requests
            .Include(r => r.Asset)
            .Include(r => r.RequestedBy)
            .Include(r => r.ApprovedBy)
            .FirstOrDefaultAsync(r => r.RequestId == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Request>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Requests
            .Include(r => r.Asset)
            .Include(r => r.RequestedBy)
            .Include(r => r.ApprovedBy)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Request>> GetByStatusAsync(RequestStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.Requests
            .Include(r => r.Asset)
            .Include(r => r.RequestedBy)
            .Where(r => r.Status == status)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Request>> GetByRequestedByAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Requests
            .Include(r => r.Asset)
            .Include(r => r.RequestedBy)
            .Include(r => r.ApprovedBy)
            .Where(r => r.RequestedById == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Request>> GetPendingByDepartmentAsync(Guid departmentId, CancellationToken cancellationToken = default)
    {
        return await _context.Requests
            .Include(r => r.Asset)
            .Include(r => r.RequestedBy)
            .Where(r => r.Status == RequestStatus.Pending &&
                        r.RequestedBy.DepartmentId == departmentId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountPendingByUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Requests
            .CountAsync(r => r.RequestedById == userId && r.Status == RequestStatus.Pending, cancellationToken);
    }

    public async Task<bool> HasActiveRequestForAssetAsync(Guid assetId, CancellationToken cancellationToken = default)
    {
        return await _context.Requests
            .AnyAsync(r => r.AssetId == assetId &&
                           (r.Status == RequestStatus.Pending || r.Status == RequestStatus.Approved),
                      cancellationToken);
    }

    public async Task<Guid> AddAsync(Request request, CancellationToken cancellationToken = default)
    {
        await _context.Requests.AddAsync(request, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return request.RequestId;
    }

    public async Task UpdateAsync(Request request, CancellationToken cancellationToken = default)
    {
        _context.Requests.Update(request);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
