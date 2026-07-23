namespace Atlas.Itam.Domain.Entities;

public sealed class AssetCategory
{
    public Guid CategoryId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private AssetCategory() { }

    public static AssetCategory Create(string name, string? description = null)
    {
        return new AssetCategory
        {
            CategoryId = Guid.NewGuid(),
            Name = name,
            Description = description,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(string name, string? description = null)
    {
        Name = name;
        Description = description;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }
}
