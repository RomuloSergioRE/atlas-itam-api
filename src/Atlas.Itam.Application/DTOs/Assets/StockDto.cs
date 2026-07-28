namespace Atlas.Itam.Application.DTOs.Assets;

public sealed class StockDto
{
    public int TotalAssets { get; set; }
    public int AvailableAssets { get; set; }
    public int InUseAssets { get; set; }
    public int InMaintenanceAssets { get; set; }
    public int RetiredAssets { get; set; }
    public decimal TotalValue { get; set; }
    public IReadOnlyList<StockByCategoryDto> ByCategory { get; set; } = [];
    public IReadOnlyList<StockByLocationDto> ByLocation { get; set; } = [];
}

public sealed class StockByCategoryDto
{
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int Total { get; set; }
    public int Available { get; set; }
    public int InUse { get; set; }
}

public sealed class StockByLocationDto
{
    public Guid LocationId { get; set; }
    public string LocationName { get; set; } = string.Empty;
    public int Total { get; set; }
}
