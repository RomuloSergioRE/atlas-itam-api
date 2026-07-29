namespace Atlas.Itam.Application.DTOs.Locations;

public sealed class LocationDto
{
    public Guid LocationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public DateTime CreatedAt { get; set; }
}
