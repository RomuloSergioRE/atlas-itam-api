namespace Atlas.Itam.Domain.Entities;

public sealed class Location
{
    public Guid LocationId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Address { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Location() { }

    public static Location Create(string name, string? address = null)
    {
        return new Location
        {
            LocationId = Guid.NewGuid(),
            Name = name,
            Address = address,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(string name, string? address = null)
    {
        Name = name;
        Address = address;
    }
}
