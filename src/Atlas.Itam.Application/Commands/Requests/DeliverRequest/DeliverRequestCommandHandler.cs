using Atlas.Itam.Application.Common.Interfaces;
using Atlas.Itam.Domain.Entities;
using Atlas.Itam.Domain.Enums;
using Atlas.Itam.Domain.Interfaces;

namespace Atlas.Itam.Application.Commands.Requests.DeliverRequest;

public sealed class DeliverRequestCommandHandler : ICommandHandler<DeliverRequestCommand>
{
    private readonly IRequestRepository _requestRepository;
    private readonly IAssetRepository _assetRepository;
    private readonly IAssetMovementRepository _movementRepository;

    public DeliverRequestCommandHandler(
        IRequestRepository requestRepository,
        IAssetRepository assetRepository,
        IAssetMovementRepository movementRepository)
    {
        _requestRepository = requestRepository;
        _assetRepository = assetRepository;
        _movementRepository = movementRepository;
    }

    public async Task Handle(DeliverRequestCommand request, CancellationToken cancellationToken)
    {
        var requestEntity = await _requestRepository.GetByIdAsync(request.RequestId, cancellationToken)
            ?? throw new Atlas.Itam.Domain.Errors.NotFoundError("Request not found");

        if (requestEntity.Status != RequestStatus.Approved)
            throw new Atlas.Itam.Domain.Errors.ConflictError("Request must be approved before delivery");

        var asset = await _assetRepository.GetByIdAsync(requestEntity.AssetId, cancellationToken)
            ?? throw new Atlas.Itam.Domain.Errors.NotFoundError("Asset not found");

        var movement = AssetMovement.Create(
            MovementType.Delivery,
            asset.AssetId,
            request.DeliveredById,
            toUserId: requestEntity.RequestedById,
            requestId: request.RequestId);

        asset.AssignToUser(requestEntity.RequestedById);
        requestEntity.Deliver();

        await _movementRepository.AddAsync(movement, cancellationToken);
        await _assetRepository.UpdateAsync(asset, cancellationToken);
        await _requestRepository.UpdateAsync(requestEntity, cancellationToken);
    }
}
