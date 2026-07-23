using Atlas.Itam.Domain.Enums;

namespace Atlas.Itam.Domain.Entities;

public sealed class Request
{
    public Guid RequestId { get; private set; }
    public RequestStatus Status { get; private set; }
    public string Justification { get; private set; } = string.Empty;
    public Guid AssetId { get; private set; }
    public Guid RequestedById { get; private set; }
    public Guid? ApprovedById { get; private set; }
    public DateTime? ApprovedAt { get; private set; }
    public string? RejectionReason { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // Navigation properties
    public Asset Asset { get; private set; } = null!;
    public User RequestedBy { get; private set; } = null!;
    public User? ApprovedBy { get; private set; }

    private Request() { }

    public static Request Create(string justification, Guid assetId, Guid requestedById)
    {
        return new Request
        {
            RequestId = Guid.NewGuid(),
            Status = RequestStatus.Pending,
            Justification = justification,
            AssetId = assetId,
            RequestedById = requestedById,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void Approve(Guid approvedById)
    {
        Status = RequestStatus.Approved;
        ApprovedById = approvedById;
        ApprovedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Reject(Guid approvedById, string reason)
    {
        Status = RequestStatus.Rejected;
        ApprovedById = approvedById;
        RejectionReason = reason;
        ApprovedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deliver()
    {
        Status = RequestStatus.Delivered;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Return()
    {
        Status = RequestStatus.Returned;
        UpdatedAt = DateTime.UtcNow;
    }
}
