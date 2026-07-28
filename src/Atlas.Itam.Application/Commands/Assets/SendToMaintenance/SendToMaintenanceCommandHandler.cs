using Atlas.Itam.Application.Common.Interfaces;
using Atlas.Itam.Domain.Entities;
using Atlas.Itam.Domain.Enums;
using Atlas.Itam.Domain.Interfaces;

namespace Atlas.Itam.Application.Commands.Assets.SendToMaintenance;

public sealed class SendToMaintenanceCommandHandler : ICommandHandler<SendToMaintenanceCommand>
{
    private readonly IAssetRepository _assetRepository;
    private readonly IAssetMovementRepository _movementRepository;

    public SendToMaintenanceCommandHandler(IAssetRepository assetRepository, IAssetMovementRepository movementRepository)
    {
        _assetRepository = assetRepository;
        _movementRepository = movementRepository;
    }

    public async Task Handle(SendToMaintenanceCommand request, CancellationToken cancellationToken)
    {
        var asset = await _assetRepository.GetByIdAsync(request.AssetId, cancellationToken)
            ?? throw new Atlas.Itam.Domain.Errors.NotFoundError("Asset not found");

        var movement = AssetMovement.Create(
            MovementType.Maintenance,
            request.AssetId,
            request.ResponsibleId,
            asset.CurrentUserId,
            observation: request.Observation);

        asset.SetInMaintenance();
        asset.UnassignFromUser();

        await _movementRepository.AddAsync(movement, cancellationToken);
        await _assetRepository.UpdateAsync(asset, cancellationToken);
    }
}
