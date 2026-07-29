using Atlas.Itam.Application.Commands.Requests.DeliverRequest;
using Atlas.Itam.Domain.Entities;
using Atlas.Itam.Domain.Enums;
using Atlas.Itam.Domain.Interfaces;
using Moq;

namespace Atlas.Itam.Application.Tests;

[TestClass]
public class DeliverRequestCommandHandlerTests
{
    private readonly Mock<IRequestRepository> _requestRepositoryMock;
    private readonly Mock<IAssetRepository> _assetRepositoryMock;
    private readonly Mock<IAssetMovementRepository> _movementRepositoryMock;
    private readonly DeliverRequestCommandHandler _handler;

    public DeliverRequestCommandHandlerTests()
    {
        _requestRepositoryMock = new Mock<IRequestRepository>();
        _assetRepositoryMock = new Mock<IAssetRepository>();
        _movementRepositoryMock = new Mock<IAssetMovementRepository>();
        _handler = new DeliverRequestCommandHandler(
            _requestRepositoryMock.Object,
            _assetRepositoryMock.Object,
            _movementRepositoryMock.Object);
    }

    [TestMethod]
    public async Task Handle_ShouldDeliverRequest_WhenApproved()
    {
        var userId = Guid.NewGuid();
        var asset = Asset.Create("Laptop", "PAT-001", "SN-001", DateTime.Now, 5000, Guid.NewGuid(), Guid.NewGuid());
        var requestEntity = Request.Create("Need it", asset.AssetId, userId);
        requestEntity.Approve(userId);

        _requestRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(requestEntity);
        _assetRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(asset);

        await _handler.Handle(new DeliverRequestCommand(requestEntity.RequestId, Guid.NewGuid()), CancellationToken.None);

        Assert.AreEqual(RequestStatus.Delivered, requestEntity.Status);
        Assert.AreEqual(userId, asset.CurrentUserId);
        Assert.AreEqual(AssetStatus.InUse, asset.Status);
        _movementRepositoryMock.Verify(x => x.AddAsync(It.IsAny<AssetMovement>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(Atlas.Itam.Domain.Errors.NotFoundError))]
    public async Task Handle_ShouldThrow_WhenRequestNotFound()
    {
        _requestRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Request?)null);

        await _handler.Handle(new DeliverRequestCommand(Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None);
    }

    [TestMethod]
    [ExpectedException(typeof(Atlas.Itam.Domain.Errors.ConflictError))]
    public async Task Handle_ShouldThrow_WhenRequestNotApproved()
    {
        var requestEntity = Request.Create("Need it", Guid.NewGuid(), Guid.NewGuid());

        _requestRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(requestEntity);

        await _handler.Handle(new DeliverRequestCommand(requestEntity.RequestId, Guid.NewGuid()), CancellationToken.None);
    }

    [TestMethod]
    [ExpectedException(typeof(Atlas.Itam.Domain.Errors.NotFoundError))]
    public async Task Handle_ShouldThrow_WhenAssetNotFound()
    {
        var requestEntity = Request.Create("Need it", Guid.NewGuid(), Guid.NewGuid());
        requestEntity.Approve(Guid.NewGuid());

        _requestRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(requestEntity);
        _assetRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Asset?)null);

        await _handler.Handle(new DeliverRequestCommand(requestEntity.RequestId, Guid.NewGuid()), CancellationToken.None);
    }
}
