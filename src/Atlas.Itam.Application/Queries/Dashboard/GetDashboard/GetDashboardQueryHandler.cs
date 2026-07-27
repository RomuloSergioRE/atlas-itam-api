using Atlas.Itam.Application.Common.Interfaces;
using Atlas.Itam.Application.DTOs.Dashboard;
using Atlas.Itam.Domain.Enums;
using Atlas.Itam.Domain.Interfaces;

namespace Atlas.Itam.Application.Queries.Dashboard.GetDashboard;

public sealed class GetDashboardQueryHandler : IQueryHandler<GetDashboardQuery, DashboardDto>
{
    private readonly IAssetRepository _assetRepository;
    private readonly IRequestRepository _requestRepository;

    public GetDashboardQueryHandler(IAssetRepository assetRepository, IRequestRepository requestRepository)
    {
        _assetRepository = assetRepository;
        _requestRepository = requestRepository;
    }

    public async Task<DashboardDto> Handle(GetDashboardQuery request, CancellationToken cancellationToken)
    {
        var allAssets = await _assetRepository.GetAllAsync(cancellationToken);
        var pendingRequests = await _requestRepository.GetByStatusAsync(RequestStatus.Pending, cancellationToken);

        var assetsByCategory = allAssets
            .GroupBy(a => a.CategoryId)
            .Select(g => new AssetsByCategoryDto
            {
                CategoryName = g.First().Category?.Name ?? "Unknown",
                Count = g.Count()
            })
            .ToList();

        var warrantyAlerts = allAssets
            .Where(a => a.WarrantyUntil.HasValue && a.WarrantyUntil.Value > DateTime.UtcNow && a.WarrantyUntil.Value <= DateTime.UtcNow.AddDays(30))
            .Select(a => new WarrantyAlertDto
            {
                AssetId = a.AssetId,
                AssetName = a.Name,
                PatrimonyNumber = a.PatrimonyNumber,
                WarrantyUntil = a.WarrantyUntil.Value,
                DaysUntilExpiration = (a.WarrantyUntil.Value - DateTime.UtcNow).Days,
                CurrentUserName = a.CurrentUser?.Name
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
            WarrantyAlerts = warrantyAlerts
        };
    }
}
