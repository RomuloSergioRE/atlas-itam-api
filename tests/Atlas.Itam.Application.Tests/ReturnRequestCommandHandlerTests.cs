using Atlas.Itam.Application.Commands.Requests.ReturnRequest;
using Atlas.Itam.Domain.Entities;
using Atlas.Itam.Domain.Enums;
using Atlas.Itam.Domain.Interfaces;
using Moq;

namespace Atlas.Itam.Application.Tests;

[TestClass]
public class ReturnRequestCommandHandlerTests
{
    private readonly Mock<IRequestRepository> _requestRepositoryMock;
    private readonly Mock<IAssetRepository> _assetRepositoryMock;
    private readonly Mock<IAssetMovementRepository> _movementRepositoryMock;
    private readonly ReturnRequestCommandHandler _handler;

    public ReturnRequestCommandHandlerTests()
    {
        _requestRepositoryMock = new Mock<IRequestRepository>();
        _assetRepositoryMock = new Mock<IAssetRepository>();
        _movementRepositoryMock = new Mock<IAssetMovementRepository>();
        _handler = new ReturnRequestCommandHandler(
            _requestRepositoryMock.Object,
            _assetRepositoryMock.Object,
            _movementRepositoryMock.Object);
    }

    [TestMethod]
    public async Task Handle_ShouldReturnRequest_WhenDelivered()
    {
        var userId = Guid.NewGuid();
        var asset = Asset.Create("Laptop", "PAT-001", "SN-001", DateTime.Now, 5000, Guid.NewGuid(), Guid.NewGuid());
        var requestEntity = Request.Create("Need it", asset.AssetId, userId);
        requestEntity.Approve(Guid.NewGuid());
        requestEntity.Deliver();
        asset.AssignToUser(userId);

        _requestRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(requestEntity);
        _assetRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(asset);

        await _handler.Handle(new ReturnRequestCommand(requestEntity.RequestId, Guid.NewGuid(), "Returned"), CancellationToken.None);

        Assert.AreEqual(RequestStatus.Returned, requestEntity.Status);
        Assert.IsNull(asset.CurrentUserId);
        Assert.AreEqual(AssetStatus.Available, asset.Status);
        _movementRepositoryMock.Verify(x => x.AddAsync(It.IsAny<AssetMovement>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(Atlas.Itam.Domain.Errors.NotFoundError))]
    public async Task Handle_ShouldThrow_WhenRequestNotFound()
    {
        _requestRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Request?)null);

        await _handler.Handle(new ReturnRequestCommand(Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None);
    }

    [TestMethod]
    [ExpectedException(typeof(Atlas.Itam.Domain.Errors.ConflictError))]
    public async Task Handle_ShouldThrow_WhenRequestNotDelivered()
    {
        var requestEntity = Request.Create("Need it", Guid.NewGuid(), Guid.NewGuid());

        _requestRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(requestEntity);

        await _handler.Handle(new ReturnRequestCommand(requestEntity.RequestId, Guid.NewGuid()), CancellationToken.None);
    }

    [TestMethod]
    [ExpectedException(typeof(Atlas.Itam.Domain.Errors.NotFoundError))]
    public async Task Handle_ShouldThrow_WhenAssetNotFound()
    {
        var requestEntity = Request.Create("Need it", Guid.NewGuid(), Guid.NewGuid());
        requestEntity.Approve(Guid.NewGuid());
        requestEntity.Deliver();

        _requestRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(requestEntity);
        _assetRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Asset?)null);

        await _handler.Handle(new ReturnRequestCommand(requestEntity.RequestId, Guid.NewGuid()), CancellationToken.None);
    }
}
