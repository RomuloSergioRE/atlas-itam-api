using Atlas.Itam.Application.Commands.Categories.CreateCategory;
using Atlas.Itam.Application.DTOs.Assets;
using Atlas.Itam.Domain.Entities;
using Atlas.Itam.Domain.Interfaces;
using AutoMapper;
using Moq;

namespace Atlas.Itam.Application.Tests;

[TestClass]
public class CreateCategoryCommandHandlerTests
{
    private readonly Mock<IAssetCategoryRepository> _categoryRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CreateCategoryCommandHandler _handler;

    public CreateCategoryCommandHandlerTests()
    {
        _categoryRepositoryMock = new Mock<IAssetCategoryRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new CreateCategoryCommandHandler(_categoryRepositoryMock.Object, _mapperMock.Object);
    }

    [TestMethod]
    public async Task Handle_ShouldCreateCategory_WhenValid()
    {
        var categoryDto = new CategoryDto { CategoryId = Guid.NewGuid(), Name = "Electronics" };

        _mapperMock.Setup(x => x.Map<CategoryDto>(It.IsAny<AssetCategory>()))
            .Returns(categoryDto);

        var result = await _handler.Handle(new CreateCategoryCommand("Electronics", "Gadgets"), CancellationToken.None);

        Assert.AreEqual("Electronics", result.Name);
        _categoryRepositoryMock.Verify(x => x.AddAsync(It.IsAny<AssetCategory>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
