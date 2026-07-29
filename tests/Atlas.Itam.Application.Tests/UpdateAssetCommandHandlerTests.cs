using Atlas.Itam.Application.Commands.Assets.UpdateAsset;
using Atlas.Itam.Application.DTOs.Assets;
using Atlas.Itam.Domain.Entities;
using Atlas.Itam.Domain.Enums;
using Atlas.Itam.Domain.Interfaces;
using AutoMapper;
using Moq;

namespace Atlas.Itam.Application.Tests;

[TestClass]
public class UpdateAssetCommandHandlerTests
{
    private readonly Mock<IAssetRepository> _assetRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly UpdateAssetCommandHandler _handler;

    public UpdateAssetCommandHandlerTests()
    {
        _assetRepositoryMock = new Mock<IAssetRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new UpdateAssetCommandHandler(_assetRepositoryMock.Object, _mapperMock.Object);
    }

    [TestMethod]
    public async Task Handle_ShouldUpdateAsset_WhenValidCommand()
    {
        var assetId = Guid.NewGuid();
        var asset = Asset.Create("Old Name", "PAT-001", "SN-001", DateTime.Now, 1000, Guid.NewGuid(), Guid.NewGuid());
        var command = new UpdateAssetCommand(assetId, "New Name", "PAT-002", "SN-002", DateTime.Now, 2000, Guid.NewGuid(), Guid.NewGuid());
        var assetDto = new AssetDto { AssetId = assetId, Name = "New Name" };

        _assetRepositoryMock.Setup(x => x.GetByIdAsync(assetId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(asset);
        _assetRepositoryMock.Setup(x => x.ExistsByPatrimonyNumberAsync(It.IsAny<string>(), It.IsAny<Guid?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _assetRepositoryMock.Setup(x => x.ExistsBySerialNumberAsync(It.IsAny<string>(), It.IsAny<Guid?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _mapperMock.Setup(x => x.Map<AssetDto>(It.IsAny<Asset>()))
            .Returns(assetDto);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.AreEqual("New Name", result.Name);
        _assetRepositoryMock.Verify(x => x.UpdateAsync(asset, It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(Atlas.Itam.Domain.Errors.NotFoundError))]
    public async Task Handle_ShouldThrowNotFound_WhenAssetDoesNotExist()
    {
        var command = new UpdateAssetCommand(Guid.NewGuid(), "Name", "PAT-001", "SN-001", DateTime.Now, 1000, Guid.NewGuid(), Guid.NewGuid());

        _assetRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Asset?)null);

        await _handler.Handle(command, CancellationToken.None);
    }

    [TestMethod]
    [ExpectedException(typeof(Atlas.Itam.Domain.Errors.ConflictError))]
    public async Task Handle_ShouldThrowConflict_WhenPatrimonyExists()
    {
        var asset = Asset.Create("Name", "PAT-001", "SN-001", DateTime.Now, 1000, Guid.NewGuid(), Guid.NewGuid());
        var command = new UpdateAssetCommand(Guid.NewGuid(), "Name", "PAT-EXISTING", "SN-001", DateTime.Now, 1000, Guid.NewGuid(), Guid.NewGuid());

        _assetRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(asset);
        _assetRepositoryMock.Setup(x => x.ExistsByPatrimonyNumberAsync("PAT-EXISTING", It.IsAny<Guid?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _handler.Handle(command, CancellationToken.None);
    }

    [TestMethod]
    [ExpectedException(typeof(Atlas.Itam.Domain.Errors.ConflictError))]
    public async Task Handle_ShouldThrowConflict_WhenSerialExists()
    {
        var asset = Asset.Create("Name", "PAT-001", "SN-001", DateTime.Now, 1000, Guid.NewGuid(), Guid.NewGuid());
        var command = new UpdateAssetCommand(Guid.NewGuid(), "Name", "PAT-001", "SN-EXISTING", DateTime.Now, 1000, Guid.NewGuid(), Guid.NewGuid());

        _assetRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(asset);
        _assetRepositoryMock.Setup(x => x.ExistsByPatrimonyNumberAsync(It.IsAny<string>(), It.IsAny<Guid?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _assetRepositoryMock.Setup(x => x.ExistsBySerialNumberAsync("SN-EXISTING", It.IsAny<Guid?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _handler.Handle(command, CancellationToken.None);
    }
}
