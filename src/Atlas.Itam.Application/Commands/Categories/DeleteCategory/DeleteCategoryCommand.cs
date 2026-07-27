using Atlas.Itam.Application.Common.Interfaces;

namespace Atlas.Itam.Application.Commands.Categories.DeleteCategory;

public sealed record DeleteCategoryCommand(Guid CategoryId) : ICommand;
