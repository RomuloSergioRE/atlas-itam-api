using Atlas.Itam.Application.Commands.Assets.SendToMaintenance;
using Atlas.Itam.Domain.Entities;
using Atlas.Itam.Domain.Enums;
using Atlas.Itam.Domain.Interfaces;
using Moq;

namespace Atlas.Itam.Application.Tests;

[TestClass]
public class SendToMaintenanceCommandHandlerTests
{
    private readonly Mock<IAssetRepository> _assetRepositoryMock;
    private readonly Mock<IAssetMovementRepository> _movementRepositoryMock;
    private readonly SendToMaintenanceCommandHandler _handler;

    public SendToMaintenanceCommandHandlerTests()
    {
        _assetRepositoryMock = new Mock<IAssetRepository>();
        _movementRepositoryMock = new Mock<IAssetMovementRepository>();
        _handler = new SendToMaintenanceCommandHandler(_assetRepositoryMock.Object, _movementRepositoryMock.Object);
    }

    [TestMethod]
    public async Task Handle_ShouldSendToMaintenance_WhenAssetIsAvailable()
    {
        var asset = Asset.Create("Asset", "PAT-001", "SN-001", DateTime.Now, 1000, Guid.NewGuid(), Guid.NewGuid());

        _assetRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(asset);

        await _handler.Handle(new SendToMaintenanceCommand(asset.AssetId, Guid.NewGuid()), CancellationToken.None);

        Assert.AreEqual(AssetStatus.InMaintenance, asset.Status);
        Assert.IsNull(asset.CurrentUserId);
        _movementRepositoryMock.Verify(x => x.AddAsync(It.IsAny<AssetMovement>(), It.IsAny<CancellationToken>()), Times.Once);
        _assetRepositoryMock.Verify(x => x.UpdateAsync(asset, It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(Atlas.Itam.Domain.Errors.NotFoundError))]
    public async Task Handle_ShouldThrowNotFound_WhenAssetDoesNotExist()
    {
        _assetRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Asset?)null);

        await _handler.Handle(new SendToMaintenanceCommand(Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None);
    }

    [TestMethod]
    [ExpectedException(typeof(Atlas.Itam.Domain.Errors.ConflictError))]
    public async Task Handle_ShouldThrowConflict_WhenAssetIsRetired()
    {
        var asset = Asset.Create("Asset", "PAT-001", "SN-001", DateTime.Now, 1000, Guid.NewGuid(), Guid.NewGuid());
        asset.Retire();

        _assetRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(asset);

        await _handler.Handle(new SendToMaintenanceCommand(asset.AssetId, Guid.NewGuid()), CancellationToken.None);
    }
}
