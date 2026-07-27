using Atlas.Itam.Application.Common.Interfaces;
using Atlas.Itam.Application.DTOs.Assets;
using Atlas.Itam.Domain.Interfaces;
using AutoMapper;

namespace Atlas.Itam.Application.Commands.Assets.UpdateAsset;

public sealed class UpdateAssetCommandHandler : ICommandHandler<UpdateAssetCommand, AssetDto>
{
    private readonly IAssetRepository _assetRepository;
    private readonly IMapper _mapper;

    public UpdateAssetCommandHandler(IAssetRepository assetRepository, IMapper mapper)
    {
        _assetRepository = assetRepository;
        _mapper = mapper;
    }

    public async Task<AssetDto> Handle(UpdateAssetCommand request, CancellationToken cancellationToken)
    {
        var asset = await _assetRepository.GetByIdAsync(request.AssetId, cancellationToken)
            ?? throw new Atlas.Itam.Domain.Errors.NotFoundError("Asset not found");

        if (await _assetRepository.ExistsByPatrimonyNumberAsync(request.PatrimonyNumber, request.AssetId, cancellationToken))
            throw new Atlas.Itam.Domain.Errors.ConflictError("An asset with this patrimony number already exists");

        if (await _assetRepository.ExistsBySerialNumberAsync(request.SerialNumber, request.AssetId, cancellationToken))
            throw new Atlas.Itam.Domain.Errors.ConflictError("An asset with this serial number already exists");

        asset.Update(
            request.Name,
            request.PatrimonyNumber,
            request.SerialNumber,
            request.AcquisitionDate,
            request.AcquisitionValue,
            request.CategoryId,
            request.LocationId,
            request.Supplier,
            request.WarrantyUntil,
            request.Observations);

        await _assetRepository.UpdateAsync(asset, cancellationToken);

        return _mapper.Map<AssetDto>(asset);
    }
}
