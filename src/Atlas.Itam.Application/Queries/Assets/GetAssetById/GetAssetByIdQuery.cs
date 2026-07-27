using Atlas.Itam.Application.Common.Interfaces;
using Atlas.Itam.Application.DTOs.Assets;

namespace Atlas.Itam.Application.Queries.Assets.GetAssetById;

public sealed record GetAssetByIdQuery(Guid AssetId) : IQuery<AssetDto>;
