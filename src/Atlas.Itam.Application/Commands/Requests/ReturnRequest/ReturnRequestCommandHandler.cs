using Atlas.Itam.Application.Common.Interfaces;
using Atlas.Itam.Domain.Entities;
using Atlas.Itam.Domain.Enums;
using Atlas.Itam.Domain.Interfaces;

namespace Atlas.Itam.Application.Commands.Requests.ReturnRequest;

public sealed class ReturnRequestCommandHandler : ICommandHandler<ReturnRequestCommand>
{
    private readonly IRequestRepository _requestRepository;
    private readonly IAssetRepository _assetRepository;
    private readonly IAssetMovementRepository _movementRepository;

    public ReturnRequestCommandHandler(
        IRequestRepository requestRepository,
        IAssetRepository assetRepository,
        IAssetMovementRepository movementRepository)
    {
        _requestRepository = requestRepository;
        _assetRepository = assetRepository;
        _movementRepository = movementRepository;
    }

    public async Task Handle(ReturnRequestCommand request, CancellationToken cancellationToken)
    {
        var requestEntity = await _requestRepository.GetByIdAsync(request.RequestId, cancellationToken)
            ?? throw new Atlas.Itam.Domain.Errors.NotFoundError("Request not found");

        if (requestEntity.Status != RequestStatus.Delivered)
            throw new Atlas.Itam.Domain.Errors.ConflictError("Request must be delivered before return");

        var asset = await _assetRepository.GetByIdAsync(requestEntity.AssetId, cancellationToken)
            ?? throw new Atlas.Itam.Domain.Errors.NotFoundError("Asset not found");

        var movement = AssetMovement.Create(
            MovementType.Return,
            asset.AssetId,
            request.ReturnedById,
            fromUserId: requestEntity.RequestedById,
            requestId: request.RequestId,
            observation: request.Observation);

        asset.UnassignFromUser();
        requestEntity.Return();

        await _movementRepository.AddAsync(movement, cancellationToken);
        await _assetRepository.UpdateAsync(asset, cancellationToken);
        await _requestRepository.UpdateAsync(requestEntity, cancellationToken);
    }
}
