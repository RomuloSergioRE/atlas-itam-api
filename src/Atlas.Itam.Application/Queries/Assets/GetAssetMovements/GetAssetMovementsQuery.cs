using Atlas.Itam.Application.Common.Interfaces;

namespace Atlas.Itam.Application.Queries.Assets.GetAssetMovements;

public sealed record GetAssetMovementsQuery(Guid AssetId) : IQuery<List<Atlas.Itam.Application.DTOs.Dashboard.RecentMovementDto>>;
