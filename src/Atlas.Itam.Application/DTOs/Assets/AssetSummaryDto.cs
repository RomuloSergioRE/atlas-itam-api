using Atlas.Itam.Domain.Enums;

namespace Atlas.Itam.Application.DTOs.Assets;

public sealed class AssetSummaryDto
{
    public Guid AssetId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string PatrimonyNumber { get; set; } = string.Empty;
    public AssetStatus Status { get; set; }
    public string? CategoryName { get; set; }
    public string? CurrentUserName { get; set; }
}
