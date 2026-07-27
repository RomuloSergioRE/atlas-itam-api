using Atlas.Itam.Application.Common.Interfaces;
using Atlas.Itam.Application.Common.Mappings;
using Atlas.Itam.Application.DTOs.Assets;
using Atlas.Itam.Domain.Enums;

namespace Atlas.Itam.Application.Queries.Assets.GetAssets;

public sealed record GetAssetsQuery(
    string? Search,
    AssetStatus? Status,
    Guid? CategoryId,
    Guid? LocationId,
    int Page = 1,
    int PageSize = 10
) : IQuery<PagedResult<AssetDto>>;
