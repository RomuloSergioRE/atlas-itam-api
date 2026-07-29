using Atlas.Itam.Application.Common.Interfaces;
using Atlas.Itam.Application.DTOs.Dashboard;
using Atlas.Itam.Domain.Enums;
using Atlas.Itam.Domain.Interfaces;

namespace Atlas.Itam.Application.Queries.Dashboard.GetDashboard;

public sealed class GetDashboardQueryHandler : IQueryHandler<GetDashboardQuery, DashboardDto>
{
    private readonly IAssetRepository _assetRepository;
    private readonly IRequestRepository _requestRepository;
    private readonly IAssetMovementRepository _movementRepository;
    private readonly IUserRepository _userRepository;

    public GetDashboardQueryHandler(
        IAssetRepository assetRepository,
        IRequestRepository requestRepository,
        IAssetMovementRepository movementRepository,
        IUserRepository userRepository)
    {
        _assetRepository = assetRepository;
        _requestRepository = requestRepository;
        _movementRepository = movementRepository;
        _userRepository = userRepository;
    }

    public async Task<DashboardDto> Handle(GetDashboardQuery request, CancellationToken cancellationToken)
    {
        var allAssets = await _assetRepository.GetAllAsync(cancellationToken);
        var pendingRequests = await _requestRepository.GetByStatusAsync(RequestStatus.Pending, cancellationToken);
        var allMovements = await _movementRepository.GetAllAsync(cancellationToken);

        var now = DateTime.UtcNow;

        var assetsByCategory = allAssets
            .GroupBy(a => a.CategoryId)
            .Select(g => new AssetsByCategoryDto
            {
                CategoryName = g.First().Category?.Name ?? "Unknown",
                Count = g.Count()
            })
            .ToList();

        var warrantyAlerts = allAssets
            .Where(a => a.WarrantyUntil.HasValue && a.WarrantyUntil.Value > now && a.WarrantyUntil.Value <= now.AddDays(90))
            .Select(a =>
            {
                var daysLeft = (a.WarrantyUntil.Value - now).Days;
                return new WarrantyAlertDto
                {
                    AssetId = a.AssetId,
                    AssetName = a.Name,
                    PatrimonyNumber = a.PatrimonyNumber,
                    WarrantyUntil = a.WarrantyUntil.Value,
                    DaysUntilExpiration = daysLeft,
                    Severity = daysLeft <= 30 ? "Critical" : daysLeft <= 60 ? "Warning" : "Info",
                    CurrentUserName = a.CurrentUser?.Name
                };
            })
            .ToList();

        var userIds = allMovements
            .SelectMany(m => new Guid?[] { m.ResponsibleId, m.FromUserId, m.ToUserId })
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .Distinct()
            .ToHashSet();

        var users = (await Task.WhenAll(userIds.Select(id => _userRepository.GetByIdAsync(id, cancellationToken))))
            .Where(u => u is not null)
            .ToDictionary(u => u!.UserId);

        var recentMovements = allMovements
            .OrderByDescending(m => m.Date)
            .Take(10)
            .Select(m =>
            {
                var asset = allAssets.FirstOrDefault(a => a.AssetId == m.AssetId);
                return new RecentMovementDto
                {
                    AssetName = asset?.Name ?? "Unknown",
                    Type = m.Type.ToString(),
                    FromUserName = m.FromUserId.HasValue && users.TryGetValue(m.FromUserId.Value, out var from) ? from.Name : null,
                    ToUserName = m.ToUserId.HasValue && users.TryGetValue(m.ToUserId.Value, out var to) ? to.Name : null,
                    Date = m.Date
                };
            })
            .ToList();

        return new DashboardDto
        {
            TotalAssets = allAssets.Count,
            AvailableAssets = allAssets.Count(a => a.Status == AssetStatus.Available),
            InUseAssets = allAssets.Count(a => a.Status == AssetStatus.InUse),
            InMaintenanceAssets = allAssets.Count(a => a.Status == AssetStatus.InMaintenance),
            PendingRequests = pendingRequests.Count,
            TotalPatrimonyValue = allAssets.Sum(a => a.AcquisitionValue),
            AssetsByCategory = assetsByCategory,
            WarrantyAlerts = warrantyAlerts,
            RecentMovements = recentMovements
        };
    }
}
