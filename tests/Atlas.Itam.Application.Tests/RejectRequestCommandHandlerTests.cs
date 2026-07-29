using Atlas.Itam.Application.Commands.Requests.RejectRequest;
using Atlas.Itam.Application.Common.Interfaces;
using Atlas.Itam.Domain.Entities;
using Atlas.Itam.Domain.Enums;
using Atlas.Itam.Domain.Interfaces;
using Moq;

namespace Atlas.Itam.Application.Tests;

[TestClass]
public class RejectRequestCommandHandlerTests
{
    private readonly Mock<IRequestRepository> _requestRepositoryMock;
    private readonly RejectRequestCommandHandler _handler;

    public RejectRequestCommandHandlerTests()
    {
        _requestRepositoryMock = new Mock<IRequestRepository>();
        _handler = new RejectRequestCommandHandler(_requestRepositoryMock.Object);
    }

    [TestMethod]
    public async Task Handle_ShouldRejectRequest_WithReason()
    {
        var requestEntity = Request.Create("Need laptop", Guid.NewGuid(), Guid.NewGuid());
        var approverId = Guid.NewGuid();

        _requestRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(requestEntity);

        await _handler.Handle(new RejectRequestCommand(requestEntity.RequestId, approverId, "Budget exceeded"), CancellationToken.None);

        Assert.AreEqual(RequestStatus.Rejected, requestEntity.Status);
        Assert.AreEqual("Budget exceeded", requestEntity.RejectionReason);
        Assert.AreEqual(approverId, requestEntity.ApprovedById);
        _requestRepositoryMock.Verify(x => x.UpdateAsync(requestEntity, It.IsAny<CancellationToken>()), Times.Once);
    }
}
