using Atlas.Itam.Domain.Enums;

namespace Atlas.Itam.Domain.Entities;

public sealed class AssetMovement
{
    public Guid MovementId { get; private set; }
    public MovementType Type { get; private set; }
    public DateTime Date { get; private set; }
    public Guid AssetId { get; private set; }
    public Guid? FromUserId { get; private set; }
    public Guid? ToUserId { get; private set; }
    public Guid ResponsibleId { get; private set; }
    public string? Observation { get; private set; }
    public Guid? RequestId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Navigation properties
    public Asset Asset { get; private set; } = null!;
    public User? FromUser { get; private set; }
    public User? ToUser { get; private set; }
    public User Responsible { get; private set; } = null!;
    public Request? Request { get; private set; }

    private AssetMovement() { }

    public static AssetMovement Create(
        MovementType type,
        Guid assetId,
        Guid responsibleId,
        Guid? fromUserId = null,
        Guid? toUserId = null,
        string? observation = null,
        Guid? requestId = null)
    {
        return new AssetMovement
        {
            MovementId = Guid.NewGuid(),
            Type = type,
            Date = DateTime.UtcNow,
            AssetId = assetId,
            FromUserId = fromUserId,
            ToUserId = toUserId,
            ResponsibleId = responsibleId,
            Observation = observation,
            RequestId = requestId,
            CreatedAt = DateTime.UtcNow
        };
    }
}
