using Atlas.Itam.Application.Common.Interfaces;
using Atlas.Itam.Application.DTOs.Dashboard;
using Atlas.Itam.Domain.Interfaces;

namespace Atlas.Itam.Application.Queries.Assets.GetAssetMovements;

public sealed class GetAssetMovementsQueryHandler : IQueryHandler<GetAssetMovementsQuery, List<RecentMovementDto>>
{
    private readonly IAssetMovementRepository _movementRepository;

    public GetAssetMovementsQueryHandler(IAssetMovementRepository movementRepository)
    {
        _movementRepository = movementRepository;
    }

    public async Task<List<RecentMovementDto>> Handle(GetAssetMovementsQuery request, CancellationToken cancellationToken)
    {
        var movements = await _movementRepository.GetByAssetIdAsync(request.AssetId, cancellationToken);

        return movements.Select(m => new RecentMovementDto
        {
            AssetName = m.Asset?.Name ?? string.Empty,
            Type = m.Type.ToString(),
            FromUserName = m.FromUser?.Name,
            ToUserName = m.ToUser?.Name,
            Date = m.CreatedAt
        }).ToList();
    }
}
