using Atlas.Itam.Application.Common.Interfaces;
using Atlas.Itam.Domain.Interfaces;

namespace Atlas.Itam.Application.Commands.Assets.DeleteAsset;

public sealed class DeleteAssetCommandHandler : ICommandHandler<DeleteAssetCommand>
{
    private readonly IAssetRepository _assetRepository;

    public DeleteAssetCommandHandler(IAssetRepository assetRepository)
    {
        _assetRepository = assetRepository;
    }

    public async Task Handle(DeleteAssetCommand request, CancellationToken cancellationToken)
    {
        var asset = await _assetRepository.GetByIdAsync(request.AssetId, cancellationToken)
            ?? throw new Atlas.Itam.Domain.Errors.NotFoundError("Asset not found");

        asset.SoftDelete();
        await _assetRepository.UpdateAsync(asset, cancellationToken);
    }
}
