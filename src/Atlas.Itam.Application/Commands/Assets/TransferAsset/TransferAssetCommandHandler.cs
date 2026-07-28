using Atlas.Itam.Application.Common.Interfaces;
using Atlas.Itam.Domain.Entities;
using Atlas.Itam.Domain.Enums;
using Atlas.Itam.Domain.Interfaces;

namespace Atlas.Itam.Application.Commands.Assets.TransferAsset;

public sealed class TransferAssetCommandHandler : ICommandHandler<TransferAssetCommand>
{
    private readonly IAssetRepository _assetRepository;
    private readonly IAssetMovementRepository _movementRepository;

    public TransferAssetCommandHandler(IAssetRepository assetRepository, IAssetMovementRepository movementRepository)
    {
        _assetRepository = assetRepository;
        _movementRepository = movementRepository;
    }

    public async Task Handle(TransferAssetCommand request, CancellationToken cancellationToken)
    {
        var asset = await _assetRepository.GetByIdAsync(request.AssetId, cancellationToken)
            ?? throw new Atlas.Itam.Domain.Errors.NotFoundError("Asset not found");

        if (asset.CurrentUserId != request.FromUserId)
            throw new Atlas.Itam.Domain.Errors.ConflictError("Asset is not assigned to the specified user");

        var movement = AssetMovement.Create(
            MovementType.Transfer,
            request.AssetId,
            request.ResponsibleId,
            request.FromUserId,
            request.ToUserId,
            request.Observation);

        asset.AssignToUser(request.ToUserId);

        await _movementRepository.AddAsync(movement, cancellationToken);
        await _assetRepository.UpdateAsync(asset, cancellationToken);
    }
}
