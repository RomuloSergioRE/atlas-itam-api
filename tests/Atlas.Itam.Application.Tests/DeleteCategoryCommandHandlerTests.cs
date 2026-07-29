using Atlas.Itam.Application.Commands.Categories.DeleteCategory;
using Atlas.Itam.Domain.Entities;
using Atlas.Itam.Domain.Interfaces;
using Moq;

namespace Atlas.Itam.Application.Tests;

[TestClass]
public class DeleteCategoryCommandHandlerTests
{
    private readonly Mock<IAssetCategoryRepository> _categoryRepositoryMock;
    private readonly DeleteCategoryCommandHandler _handler;

    public DeleteCategoryCommandHandlerTests()
    {
        _categoryRepositoryMock = new Mock<IAssetCategoryRepository>();
        _handler = new DeleteCategoryCommandHandler(_categoryRepositoryMock.Object);
    }

    [TestMethod]
    public async Task Handle_ShouldDeleteCategory_WhenNoAssets()
    {
        var category = AssetCategory.Create("Category", "Desc");

        _categoryRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);
        _categoryRepositoryMock.Setup(x => x.HasAssetsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        await _handler.Handle(new DeleteCategoryCommand(category.CategoryId), CancellationToken.None);

        _categoryRepositoryMock.Verify(x => x.DeleteAsync(category, It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(Atlas.Itam.Domain.Errors.NotFoundError))]
    public async Task Handle_ShouldThrow_WhenCategoryNotFound()
    {
        _categoryRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((AssetCategory?)null);

        await _handler.Handle(new DeleteCategoryCommand(Guid.NewGuid()), CancellationToken.None);
    }

    [TestMethod]
    [ExpectedException(typeof(Atlas.Itam.Domain.Errors.ConflictError))]
    public async Task Handle_ShouldThrow_WhenCategoryHasAssets()
    {
        var category = AssetCategory.Create("Category", "Desc");

        _categoryRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);
        _categoryRepositoryMock.Setup(x => x.HasAssetsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _handler.Handle(new DeleteCategoryCommand(category.CategoryId), CancellationToken.None);
    }
}
