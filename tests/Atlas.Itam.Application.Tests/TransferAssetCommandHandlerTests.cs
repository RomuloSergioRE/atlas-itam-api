using Atlas.Itam.Application.Commands.Assets.TransferAsset;
using Atlas.Itam.Domain.Entities;
using Atlas.Itam.Domain.Enums;
using Atlas.Itam.Domain.Interfaces;
using Moq;

namespace Atlas.Itam.Application.Tests;

[TestClass]
public class TransferAssetCommandHandlerTests
{
    private readonly Mock<IAssetRepository> _assetRepositoryMock;
    private readonly Mock<IAssetMovementRepository> _movementRepositoryMock;
    private readonly TransferAssetCommandHandler _handler;

    public TransferAssetCommandHandlerTests()
    {
        _assetRepositoryMock = new Mock<IAssetRepository>();
        _movementRepositoryMock = new Mock<IAssetMovementRepository>();
        _handler = new TransferAssetCommandHandler(_assetRepositoryMock.Object, _movementRepositoryMock.Object);
    }

    [TestMethod]
    public async Task Handle_ShouldTransferAsset_WhenValid()
    {
        var fromUserId = Guid.NewGuid();
        var toUserId = Guid.NewGuid();
        var asset = Asset.Create("Asset", "PAT-001", "SN-001", DateTime.Now, 1000, Guid.NewGuid(), Guid.NewGuid());
        asset.AssignToUser(fromUserId);

        _assetRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(asset);

        await _handler.Handle(new TransferAssetCommand(asset.AssetId, fromUserId, toUserId, Guid.NewGuid()), CancellationToken.None);

        Assert.AreEqual(toUserId, asset.CurrentUserId);
        Assert.AreEqual(AssetStatus.InUse, asset.Status);
        _movementRepositoryMock.Verify(x => x.AddAsync(It.IsAny<AssetMovement>(), It.IsAny<CancellationToken>()), Times.Once);
        _assetRepositoryMock.Verify(x => x.UpdateAsync(asset, It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(Atlas.Itam.Domain.Errors.NotFoundError))]
    public async Task Handle_ShouldThrowNotFound_WhenAssetDoesNotExist()
    {
        _assetRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Asset?)null);

        await _handler.Handle(new TransferAssetCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None);
    }

    [TestMethod]
    [ExpectedException(typeof(Atlas.Itam.Domain.Errors.ConflictError))]
    public async Task Handle_ShouldThrowConflict_WhenAssetIsRetired()
    {
        var asset = Asset.Create("Asset", "PAT-001", "SN-001", DateTime.Now, 1000, Guid.NewGuid(), Guid.NewGuid());
        asset.Retire();

        _assetRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(asset);

        await _handler.Handle(new TransferAssetCommand(asset.AssetId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None);
    }

    [TestMethod]
    [ExpectedException(typeof(Atlas.Itam.Domain.Errors.ConflictError))]
    public async Task Handle_ShouldThrowConflict_WhenAssetNotAssignedToFromUser()
    {
        var asset = Asset.Create("Asset", "PAT-001", "SN-001", DateTime.Now, 1000, Guid.NewGuid(), Guid.NewGuid());
        asset.AssignToUser(Guid.NewGuid());

        _assetRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(asset);

        await _handler.Handle(new TransferAssetCommand(asset.AssetId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None);
    }
}
