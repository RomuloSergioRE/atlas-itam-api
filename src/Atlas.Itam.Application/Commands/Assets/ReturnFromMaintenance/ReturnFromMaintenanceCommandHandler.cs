using Atlas.Itam.Application.Common.Interfaces;
using Atlas.Itam.Domain.Entities;
using Atlas.Itam.Domain.Enums;
using Atlas.Itam.Domain.Interfaces;

namespace Atlas.Itam.Application.Commands.Assets.ReturnFromMaintenance;

public sealed class ReturnFromMaintenanceCommandHandler : ICommandHandler<ReturnFromMaintenanceCommand>
{
    private readonly IAssetRepository _assetRepository;
    private readonly IAssetMovementRepository _movementRepository;

    public ReturnFromMaintenanceCommandHandler(IAssetRepository assetRepository, IAssetMovementRepository movementRepository)
    {
        _assetRepository = assetRepository;
        _movementRepository = movementRepository;
    }

    public async Task Handle(ReturnFromMaintenanceCommand request, CancellationToken cancellationToken)
    {
        var asset = await _assetRepository.GetByIdAsync(request.AssetId, cancellationToken)
            ?? throw new Atlas.Itam.Domain.Errors.NotFoundError("Asset not found");

        if (asset.Status != AssetStatus.InMaintenance)
            throw new Atlas.Itam.Domain.Errors.ConflictError("Asset is not in maintenance");

        var movement = AssetMovement.Create(
            MovementType.Return,
            request.AssetId,
            request.ResponsibleId,
            observation: request.Observation);

        asset.ReturnFromMaintenance();

        await _movementRepository.AddAsync(movement, cancellationToken);
        await _assetRepository.UpdateAsync(asset, cancellationToken);
    }
}
