using Atlas.Itam.Application.Commands.Assets.DeleteAsset;
using Atlas.Itam.Domain.Entities;
using Atlas.Itam.Domain.Interfaces;
using Moq;

namespace Atlas.Itam.Application.Tests;

[TestClass]
public class DeleteAssetCommandHandlerTests
{
    private readonly Mock<IAssetRepository> _assetRepositoryMock;
    private readonly DeleteAssetCommandHandler _handler;

    public DeleteAssetCommandHandlerTests()
    {
        _assetRepositoryMock = new Mock<IAssetRepository>();
        _handler = new DeleteAssetCommandHandler(_assetRepositoryMock.Object);
    }

    [TestMethod]
    public async Task Handle_ShouldSoftDeleteAsset_WhenAssetExists()
    {
        var asset = Asset.Create("Asset", "PAT-001", "SN-001", DateTime.Now, 1000, Guid.NewGuid(), Guid.NewGuid());

        _assetRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(asset);

        await _handler.Handle(new DeleteAssetCommand(asset.AssetId), CancellationToken.None);

        Assert.IsTrue(asset.IsDeleted);
        _assetRepositoryMock.Verify(x => x.UpdateAsync(asset, It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(Atlas.Itam.Domain.Errors.NotFoundError))]
    public async Task Handle_ShouldThrowNotFound_WhenAssetDoesNotExist()
    {
        _assetRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Asset?)null);

        await _handler.Handle(new DeleteAssetCommand(Guid.NewGuid()), CancellationToken.None);
    }
}
