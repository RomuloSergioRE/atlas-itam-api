using Atlas.Itam.Application.Commands.Categories.UpdateCategory;
using Atlas.Itam.Application.DTOs.Assets;
using Atlas.Itam.Domain.Entities;
using Atlas.Itam.Domain.Interfaces;
using AutoMapper;
using Moq;

namespace Atlas.Itam.Application.Tests;

[TestClass]
public class UpdateCategoryCommandHandlerTests
{
    private readonly Mock<IAssetCategoryRepository> _categoryRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly UpdateCategoryCommandHandler _handler;

    public UpdateCategoryCommandHandlerTests()
    {
        _categoryRepositoryMock = new Mock<IAssetCategoryRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new UpdateCategoryCommandHandler(_categoryRepositoryMock.Object, _mapperMock.Object);
    }

    [TestMethod]
    public async Task Handle_ShouldUpdateCategory_WhenCategoryExists()
    {
        var category = AssetCategory.Create("Old Name", "Old desc");
        var categoryDto = new CategoryDto { CategoryId = category.CategoryId, Name = "New Name" };

        _categoryRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);
        _mapperMock.Setup(x => x.Map<CategoryDto>(It.IsAny<AssetCategory>()))
            .Returns(categoryDto);

        var result = await _handler.Handle(new UpdateCategoryCommand(category.CategoryId, "New Name", "New desc"), CancellationToken.None);

        Assert.AreEqual("New Name", result.Name);
        _categoryRepositoryMock.Verify(x => x.UpdateAsync(category, It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(Atlas.Itam.Domain.Errors.NotFoundError))]
    public async Task Handle_ShouldThrowNotFound_WhenCategoryDoesNotExist()
    {
        _categoryRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((AssetCategory?)null);

        await _handler.Handle(new UpdateCategoryCommand(Guid.NewGuid(), "Name", null), CancellationToken.None);
    }
}
