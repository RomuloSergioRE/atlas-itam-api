using Atlas.Itam.Domain.Enums;

namespace Atlas.Itam.Application.DTOs.Dashboard;

public sealed class AuditLogDto
{
    public Guid LogId { get; set; }
    public Guid UserId { get; set; }
    public string? UserName { get; set; }
    public AuditAction Action { get; set; }
    public string EntityName { get; set; } = string.Empty;
    public Guid EntityId { get; set; }
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public string? IpAddress { get; set; }
    public DateTime CreatedAt { get; set; }
}
