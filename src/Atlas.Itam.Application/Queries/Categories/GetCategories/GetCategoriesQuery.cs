using Atlas.Itam.Application.Common.Interfaces;
using Atlas.Itam.Application.DTOs.Assets;

namespace Atlas.Itam.Application.Queries.Categories.GetCategories;

public sealed record GetCategoriesQuery(bool ActiveOnly = false) : IQuery<IReadOnlyList<CategoryDto>>;
