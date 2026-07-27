using Atlas.Itam.Domain.Enums;

namespace Atlas.Itam.Application.DTOs.Requests;

public sealed class RequestDto
{
    public Guid RequestId { get; set; }
    public RequestStatus Status { get; set; }
    public string Justification { get; set; } = string.Empty;
    public Guid AssetId { get; set; }
    public string? AssetName { get; set; }
    public string? AssetPatrimonyNumber { get; set; }
    public Guid RequestedById { get; set; }
    public string? RequestedByName { get; set; }
    public string? RequestedByEmail { get; set; }
    public Guid? ApprovedById { get; set; }
    public string? ApprovedByName { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? RejectionReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
