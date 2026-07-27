using Atlas.Itam.Domain.Enums;

namespace Atlas.Itam.Application.DTOs.Assets;

public sealed class AssetDto
{
    public Guid AssetId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string PatrimonyNumber { get; set; } = string.Empty;
    public string SerialNumber { get; set; } = string.Empty;
    public DateTime AcquisitionDate { get; set; }
    public decimal AcquisitionValue { get; set; }
    public string? Supplier { get; set; }
    public DateTime? WarrantyUntil { get; set; }
    public AssetStatus Status { get; set; }
    public string? Observations { get; set; }
    public Guid CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public Guid LocationId { get; set; }
    public string? LocationName { get; set; }
    public Guid? CurrentUserId { get; set; }
    public string? CurrentUserName { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
