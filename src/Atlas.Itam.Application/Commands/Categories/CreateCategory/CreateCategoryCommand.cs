using Atlas.Itam.Application.Common.Interfaces;
using Atlas.Itam.Application.DTOs.Assets;

namespace Atlas.Itam.Application.Commands.Categories.CreateCategory;

public sealed record CreateCategoryCommand(
    string Name,
    string? Description = null
) : ICommand<CategoryDto>;
