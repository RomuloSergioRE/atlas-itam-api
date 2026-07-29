using Atlas.Itam.Application.Commands.Assets.ReturnFromMaintenance;
using Atlas.Itam.Domain.Entities;
using Atlas.Itam.Domain.Enums;
using Atlas.Itam.Domain.Interfaces;
using Moq;

namespace Atlas.Itam.Application.Tests;

[TestClass]
public class ReturnFromMaintenanceCommandHandlerTests
{
    private readonly Mock<IAssetRepository> _assetRepositoryMock;
    private readonly Mock<IAssetMovementRepository> _movementRepositoryMock;
    private readonly ReturnFromMaintenanceCommandHandler _handler;

    public ReturnFromMaintenanceCommandHandlerTests()
    {
        _assetRepositoryMock = new Mock<IAssetRepository>();
        _movementRepositoryMock = new Mock<IAssetMovementRepository>();
        _handler = new ReturnFromMaintenanceCommandHandler(_assetRepositoryMock.Object, _movementRepositoryMock.Object);
    }

    [TestMethod]
    public async Task Handle_ShouldReturnFromMaintenance_WhenAssetIsInMaintenance()
    {
        var asset = Asset.Create("Asset", "PAT-001", "SN-001", DateTime.Now, 1000, Guid.NewGuid(), Guid.NewGuid());
        asset.SetInMaintenance();

        _assetRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(asset);

        await _handler.Handle(new ReturnFromMaintenanceCommand(asset.AssetId, Guid.NewGuid()), CancellationToken.None);

        Assert.AreEqual(AssetStatus.Available, asset.Status);
        _movementRepositoryMock.Verify(x => x.AddAsync(It.IsAny<AssetMovement>(), It.IsAny<CancellationToken>()), Times.Once);
        _assetRepositoryMock.Verify(x => x.UpdateAsync(asset, It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(Atlas.Itam.Domain.Errors.NotFoundError))]
    public async Task Handle_ShouldThrowNotFound_WhenAssetDoesNotExist()
    {
        _assetRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Asset?)null);

        await _handler.Handle(new ReturnFromMaintenanceCommand(Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None);
    }

    [TestMethod]
    [ExpectedException(typeof(Atlas.Itam.Domain.Errors.ConflictError))]
    public async Task Handle_ShouldThrowConflict_WhenAssetNotInMaintenance()
    {
        var asset = Asset.Create("Asset", "PAT-001", "SN-001", DateTime.Now, 1000, Guid.NewGuid(), Guid.NewGuid());

        _assetRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(asset);

        await _handler.Handle(new ReturnFromMaintenanceCommand(asset.AssetId, Guid.NewGuid()), CancellationToken.None);
    }
}
