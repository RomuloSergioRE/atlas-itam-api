using Atlas.Itam.Domain.Enums;

namespace Atlas.Itam.Application.DTOs.Requests;

public sealed class RequestSummaryDto
{
    public Guid RequestId { get; set; }
    public RequestStatus Status { get; set; }
    public string? AssetName { get; set; }
    public string? RequestedByName { get; set; }
    public DateTime CreatedAt { get; set; }
}
