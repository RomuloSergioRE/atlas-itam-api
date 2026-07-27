using Atlas.Itam.Application.Common.Interfaces;
using Atlas.Itam.Domain.Interfaces;

namespace Atlas.Itam.Application.Commands.Categories.DeleteCategory;

public sealed class DeleteCategoryCommandHandler : ICommandHandler<DeleteCategoryCommand>
{
    private readonly IAssetCategoryRepository _categoryRepository;

    public DeleteCategoryCommandHandler(IAssetCategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken)
            ?? throw new Atlas.Itam.Domain.Errors.NotFoundError("Category not found");

        if (await _categoryRepository.HasAssetsAsync(request.CategoryId, cancellationToken))
            throw new Atlas.Itam.Domain.Errors.ConflictError("Cannot delete category with existing assets");

        await _categoryRepository.DeleteAsync(category, cancellationToken);
    }
}
