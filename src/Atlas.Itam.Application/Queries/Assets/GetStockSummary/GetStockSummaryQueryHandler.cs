using Atlas.Itam.Application.Common.Interfaces;
using Atlas.Itam.Application.DTOs.Assets;
using Atlas.Itam.Domain.Enums;
using Atlas.Itam.Domain.Interfaces;

namespace Atlas.Itam.Application.Queries.Assets.GetStockSummary;

public sealed class GetStockSummaryQueryHandler : IQueryHandler<GetStockSummaryQuery, StockDto>
{
    private readonly IAssetRepository _assetRepository;

    public GetStockSummaryQueryHandler(IAssetRepository assetRepository)
    {
        _assetRepository = assetRepository;
    }

    public async Task<StockDto> Handle(GetStockSummaryQuery request, CancellationToken cancellationToken)
    {
        var assets = await _assetRepository.GetAllAsync(cancellationToken);

        var byCategory = assets
            .GroupBy(a => new { a.CategoryId, CategoryName = a.Category?.Name ?? "Unknown" })
            .Select(g => new StockByCategoryDto
            {
                CategoryId = g.Key.CategoryId,
                CategoryName = g.Key.CategoryName,
                Total = g.Count(),
                Available = g.Count(a => a.Status == AssetStatus.Available),
                InUse = g.Count(a => a.Status == AssetStatus.InUse)
            })
            .OrderByDescending(x => x.Total)
            .ToList();

        var byLocation = assets
            .GroupBy(a => new { a.LocationId, LocationName = a.Location?.Name ?? "Unknown" })
            .Select(g => new StockByLocationDto
            {
                LocationId = g.Key.LocationId,
                LocationName = g.Key.LocationName,
                Total = g.Count()
            })
            .OrderByDescending(x => x.Total)
            .ToList();

        return new StockDto
        {
            TotalAssets = assets.Count,
            AvailableAssets = assets.Count(a => a.Status == AssetStatus.Available),
            InUseAssets = assets.Count(a => a.Status == AssetStatus.InUse),
            InMaintenanceAssets = assets.Count(a => a.Status == AssetStatus.InMaintenance),
            RetiredAssets = assets.Count(a => a.Status == AssetStatus.Retired),
            TotalValue = assets.Where(a => a.Status != AssetStatus.Retired).Sum(a => a.AcquisitionValue),
            ByCategory = byCategory,
            ByLocation = byLocation
        };
    }
}
