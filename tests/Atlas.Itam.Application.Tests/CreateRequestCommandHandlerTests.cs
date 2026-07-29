using Atlas.Itam.Application.Commands.Requests.CreateRequest;
using Atlas.Itam.Application.DTOs.Requests;
using Atlas.Itam.Domain.Entities;
using Atlas.Itam.Domain.Enums;
using Atlas.Itam.Domain.Interfaces;
using AutoMapper;
using Moq;

namespace Atlas.Itam.Application.Tests;

[TestClass]
public class CreateRequestCommandHandlerTests
{
    private readonly Mock<IRequestRepository> _requestRepositoryMock;
    private readonly Mock<IAssetRepository> _assetRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CreateRequestCommandHandler _handler;

    public CreateRequestCommandHandlerTests()
    {
        _requestRepositoryMock = new Mock<IRequestRepository>();
        _assetRepositoryMock = new Mock<IAssetRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new CreateRequestCommandHandler(
            _requestRepositoryMock.Object,
            _assetRepositoryMock.Object,
            _mapperMock.Object);
    }

    [TestMethod]
    public async Task Handle_ShouldCreateRequest_WhenValid()
    {
        var userId = Guid.NewGuid();
        var asset = Asset.Create("Laptop", "PAT-001", "SN-001", DateTime.Now, 5000, Guid.NewGuid(), Guid.NewGuid());
        var requestDto = new RequestDto { RequestId = Guid.NewGuid(), Justification = "Need it" };

        _requestRepositoryMock.Setup(x => x.CountPendingByUserAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
        _assetRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(asset);
        _requestRepositoryMock.Setup(x => x.HasActiveRequestForAssetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _mapperMock.Setup(x => x.Map<RequestDto>(It.IsAny<Request>()))
            .Returns(requestDto);

        var result = await _handler.Handle(
            new CreateRequestCommand("Need it", asset.AssetId, userId), CancellationToken.None);

        Assert.IsNotNull(result);
        Assert.AreEqual("Need it", result.Justification);
        _requestRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Request>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(Atlas.Itam.Domain.Errors.ConflictError))]
    public async Task Handle_ShouldThrow_WhenMaxPendingReached()
    {
        var userId = Guid.NewGuid();

        _requestRepositoryMock.Setup(x => x.CountPendingByUserAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(3);

        await _handler.Handle(
            new CreateRequestCommand("Need it", Guid.NewGuid(), userId), CancellationToken.None);
    }

    [TestMethod]
    [ExpectedException(typeof(Atlas.Itam.Domain.Errors.NotFoundError))]
    public async Task Handle_ShouldThrow_WhenAssetNotFound()
    {
        _requestRepositoryMock.Setup(x => x.CountPendingByUserAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
        _assetRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Asset?)null);

        await _handler.Handle(
            new CreateRequestCommand("Need it", Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None);
    }

    [TestMethod]
    [ExpectedException(typeof(Atlas.Itam.Domain.Errors.ConflictError))]
    public async Task Handle_ShouldThrow_WhenAssetAlreadyAssignedToUser()
    {
        var userId = Guid.NewGuid();
        var asset = Asset.Create("Laptop", "PAT-001", "SN-001", DateTime.Now, 5000, Guid.NewGuid(), Guid.NewGuid());
        asset.AssignToUser(userId);

        _requestRepositoryMock.Setup(x => x.CountPendingByUserAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
        _assetRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(asset);

        await _handler.Handle(
            new CreateRequestCommand("Need it", asset.AssetId, userId), CancellationToken.None);
    }

    [TestMethod]
    [ExpectedException(typeof(Atlas.Itam.Domain.Errors.ConflictError))]
    public async Task Handle_ShouldThrow_WhenAssetNotAvailable()
    {
        var userId = Guid.NewGuid();
        var asset = Asset.Create("Laptop", "PAT-001", "SN-001", DateTime.Now, 5000, Guid.NewGuid(), Guid.NewGuid());
        asset.AssignToUser(Guid.NewGuid());

        _requestRepositoryMock.Setup(x => x.CountPendingByUserAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
        _assetRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(asset);

        await _handler.Handle(
            new CreateRequestCommand("Need it", asset.AssetId, userId), CancellationToken.None);
    }

    [TestMethod]
    [ExpectedException(typeof(Atlas.Itam.Domain.Errors.ConflictError))]
    public async Task Handle_ShouldThrow_WhenActiveRequestExists()
    {
        var userId = Guid.NewGuid();
        var asset = Asset.Create("Laptop", "PAT-001", "SN-001", DateTime.Now, 5000, Guid.NewGuid(), Guid.NewGuid());

        _requestRepositoryMock.Setup(x => x.CountPendingByUserAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
        _assetRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(asset);
        _requestRepositoryMock.Setup(x => x.HasActiveRequestForAssetAsync(asset.AssetId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _handler.Handle(
            new CreateRequestCommand("Need it", asset.AssetId, userId), CancellationToken.None);
    }
}
