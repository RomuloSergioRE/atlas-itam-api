using Atlas.Itam.Domain.Enums;

namespace Atlas.Itam.Domain.Entities;

public sealed class Asset
{
    public Guid AssetId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string PatrimonyNumber { get; private set; } = string.Empty;
    public string SerialNumber { get; private set; } = string.Empty;
    public DateTime AcquisitionDate { get; private set; }
    public decimal AcquisitionValue { get; private set; }
    public string? Supplier { get; private set; }
    public DateTime? WarrantyUntil { get; private set; }
    public AssetStatus Status { get; private set; }
    public string? Observations { get; private set; }
    public Guid CategoryId { get; private set; }
    public Guid LocationId { get; private set; }
    public Guid? CurrentUserId { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // Navigation properties
    public AssetCategory Category { get; private set; } = null!;
    public Location Location { get; private set; } = null!;
    public User? CurrentUser { get; private set; }

    private Asset() { }

    public static Asset Create(
        string name,
        string patrimonyNumber,
        string serialNumber,
        DateTime acquisitionDate,
        decimal acquisitionValue,
        Guid categoryId,
        Guid locationId,
        string? supplier = null,
        DateTime? warrantyUntil = null,
        string? observations = null)
    {
        return new Asset
        {
            AssetId = Guid.NewGuid(),
            Name = name,
            PatrimonyNumber = patrimonyNumber,
            SerialNumber = serialNumber,
            AcquisitionDate = acquisitionDate,
            AcquisitionValue = acquisitionValue,
            Supplier = supplier,
            WarrantyUntil = warrantyUntil,
            Status = AssetStatus.Available,
            Observations = observations,
            CategoryId = categoryId,
            LocationId = locationId,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void Update(
        string name,
        string patrimonyNumber,
        string serialNumber,
        DateTime acquisitionDate,
        decimal acquisitionValue,
        Guid categoryId,
        Guid locationId,
        string? supplier = null,
        DateTime? warrantyUntil = null,
        string? observations = null)
    {
        Name = name;
        PatrimonyNumber = patrimonyNumber;
        SerialNumber = serialNumber;
        AcquisitionDate = acquisitionDate;
        AcquisitionValue = acquisitionValue;
        Supplier = supplier;
        WarrantyUntil = warrantyUntil;
        Observations = observations;
        CategoryId = categoryId;
        LocationId = locationId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AssignToUser(Guid userId)
    {
        CurrentUserId = userId;
        Status = AssetStatus.InUse;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UnassignFromUser()
    {
        CurrentUserId = null;
        Status = AssetStatus.Available;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetInMaintenance()
    {
        Status = AssetStatus.InMaintenance;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ReturnFromMaintenance()
    {
        Status = AssetStatus.Available;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Retire()
    {
        Status = AssetStatus.Retired;
        CurrentUserId = null;
        UpdatedAt = DateTime.UtcNow;
    }
}
