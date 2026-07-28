using Atlas.Itam.Application.Commands.Assets.CreateAsset;
using Atlas.Itam.Application.Common.Interfaces;
using Atlas.Itam.Application.DTOs.Assets;
using Atlas.Itam.Domain.Entities;
using Atlas.Itam.Domain.Enums;
using Atlas.Itam.Domain.Interfaces;
using AutoMapper;
using Moq;

namespace Atlas.Itam.Application.Tests;

[TestClass]
public class CreateAssetCommandHandlerTests
{
    private readonly Mock<IAssetRepository> _assetRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CreateAssetCommandHandler _handler;

    public CreateAssetCommandHandlerTests()
    {
        _assetRepositoryMock = new Mock<IAssetRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new CreateAssetCommandHandler(_assetRepositoryMock.Object, _mapperMock.Object);
    }

    [TestMethod]
    public async Task Handle_ShouldCreateAsset_WhenValidCommand()
    {
        var command = new CreateAssetCommand("Dell XPS 15", "PAT-001", "SN-12345", DateTime.Now, 5999.99m, Guid.NewGuid(), Guid.NewGuid());

        _assetRepositoryMock.Setup(x => x.ExistsByPatrimonyNumberAsync(It.IsAny<string>(), It.IsAny<Guid?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _assetRepositoryMock.Setup(x => x.ExistsBySerialNumberAsync(It.IsAny<string>(), It.IsAny<Guid?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var assetDto = new AssetDto { AssetId = Guid.NewGuid(), Name = command.Name };
        _mapperMock.Setup(x => x.Map<AssetDto>(It.IsAny<Asset>()))
            .Returns(assetDto);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.IsNotNull(result);
        Assert.AreEqual("Dell XPS 15", result.Name);
        _assetRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Asset>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(Atlas.Itam.Domain.Errors.ConflictError))]
    public async Task Handle_ShouldThrowConflict_WhenPatrimonyExists()
    {
        var command = new CreateAssetCommand("Test", "PAT-001", "SN-12345", DateTime.Now, 1000, Guid.NewGuid(), Guid.NewGuid());

        _assetRepositoryMock.Setup(x => x.ExistsByPatrimonyNumberAsync(It.IsAny<string>(), It.IsAny<Guid?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _handler.Handle(command, CancellationToken.None);
    }

    [TestMethod]
    [ExpectedException(typeof(Atlas.Itam.Domain.Errors.ConflictError))]
    public async Task Handle_ShouldThrowConflict_WhenSerialExists()
    {
        var command = new CreateAssetCommand("Test", "PAT-001", "SN-12345", DateTime.Now, 1000, Guid.NewGuid(), Guid.NewGuid());

        _assetRepositoryMock.Setup(x => x.ExistsByPatrimonyNumberAsync(It.IsAny<string>(), It.IsAny<Guid?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _assetRepositoryMock.Setup(x => x.ExistsBySerialNumberAsync(It.IsAny<string>(), It.IsAny<Guid?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _handler.Handle(command, CancellationToken.None);
    }
}
