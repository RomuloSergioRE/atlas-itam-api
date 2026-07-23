using Atlas.Itam.Domain.Enums;

namespace Atlas.Itam.Domain.Entities;

public sealed class AuditLog
{
    public Guid LogId { get; private set; }
    public Guid UserId { get; private set; }
    public AuditAction Action { get; private set; }
    public string EntityName { get; private set; } = string.Empty;
    public Guid EntityId { get; private set; }
    public string? OldValues { get; private set; }
    public string? NewValues { get; private set; }
    public string? IpAddress { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Navigation properties
    public User User { get; private set; } = null!;

    private AuditLog() { }

    public static AuditLog Create(
        Guid userId,
        AuditAction action,
        string entityName,
        Guid entityId,
        string? oldValues = null,
        string? newValues = null,
        string? ipAddress = null)
    {
        return new AuditLog
        {
            LogId = Guid.NewGuid(),
            UserId = userId,
            Action = action,
            EntityName = entityName,
            EntityId = entityId,
            OldValues = oldValues,
            NewValues = newValues,
            IpAddress = ipAddress,
            CreatedAt = DateTime.UtcNow
        };
    }
}
