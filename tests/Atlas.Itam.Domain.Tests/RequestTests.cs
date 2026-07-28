using Atlas.Itam.Domain.Entities;
using Atlas.Itam.Domain.Enums;

namespace Atlas.Itam.Domain.Tests;

[TestClass]
public class RequestTests
{
    [TestMethod]
    public void Create_ShouldSetPendingStatus()
    {
        var assetId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var request = Request.Create("Need laptop for project", assetId, userId);

        Assert.AreEqual(RequestStatus.Pending, request.Status);
        Assert.AreEqual("Need laptop for project", request.Justification);
        Assert.AreEqual(assetId, request.AssetId);
        Assert.AreEqual(userId, request.RequestedById);
        Assert.IsNull(request.ApprovedById);
    }

    [TestMethod]
    public void Approve_ShouldSetStatusAndApprover()
    {
        var request = Request.Create("Need laptop", Guid.NewGuid(), Guid.NewGuid());
        var approverId = Guid.NewGuid();

        request.Approve(approverId);

        Assert.AreEqual(RequestStatus.Approved, request.Status);
        Assert.AreEqual(approverId, request.ApprovedById);
        Assert.IsNotNull(request.ApprovedAt);
    }

    [TestMethod]
    public void Reject_ShouldSetStatusAndReason()
    {
        var request = Request.Create("Need laptop", Guid.NewGuid(), Guid.NewGuid());
        var approverId = Guid.NewGuid();

        request.Reject(approverId, "Budget exceeded");

        Assert.AreEqual(RequestStatus.Rejected, request.Status);
        Assert.AreEqual("Budget exceeded", request.RejectionReason);
        Assert.AreEqual(approverId, request.ApprovedById);
    }

    [TestMethod]
    public void Deliver_ShouldSetStatusToDelivered()
    {
        var request = Request.Create("Need laptop", Guid.NewGuid(), Guid.NewGuid());
        request.Approve(Guid.NewGuid());

        request.Deliver();

        Assert.AreEqual(RequestStatus.Delivered, request.Status);
    }

    [TestMethod]
    public void Return_ShouldSetStatusToReturned()
    {
        var request = Request.Create("Need laptop", Guid.NewGuid(), Guid.NewGuid());
        request.Approve(Guid.NewGuid());
        request.Deliver();

        request.Return();

        Assert.AreEqual(RequestStatus.Returned, request.Status);
    }
}
