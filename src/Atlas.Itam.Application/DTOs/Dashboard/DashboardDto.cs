namespace Atlas.Itam.Application.DTOs.Dashboard;

public sealed class DashboardDto
{
    public int TotalAssets { get; set; }
    public int AvailableAssets { get; set; }
    public int InUseAssets { get; set; }
    public int InMaintenanceAssets { get; set; }
    public int PendingRequests { get; set; }
    public decimal TotalPatrimonyValue { get; set; }
    public IReadOnlyList<AssetsByCategoryDto> AssetsByCategory { get; set; } = [];
    public IReadOnlyList<WarrantyAlertDto> WarrantyAlerts { get; set; } = [];
    public IReadOnlyList<RecentMovementDto> RecentMovements { get; set; } = [];
}

public sealed class AssetsByCategoryDto
{
    public string CategoryName { get; set; } = string.Empty;
    public int Count { get; set; }
}

public sealed class WarrantyAlertDto
{
    public Guid AssetId { get; set; }
    public string AssetName { get; set; } = string.Empty;
    public string PatrimonyNumber { get; set; } = string.Empty;
    public DateTime WarrantyUntil { get; set; }
    public int DaysUntilExpiration { get; set; }
    public string Severity { get; set; } = string.Empty;
    public string? CurrentUserName { get; set; }
}

public sealed class RecentMovementDto
{
    public string AssetName { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? FromUserName { get; set; }
    public string? ToUserName { get; set; }
    public DateTime Date { get; set; }
}
