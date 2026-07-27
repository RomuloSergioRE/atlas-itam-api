using Atlas.Itam.Application.Common.Interfaces;
using Atlas.Itam.Application.DTOs.Assets;
using Atlas.Itam.Domain.Entities;
using Atlas.Itam.Domain.Interfaces;
using AutoMapper;

namespace Atlas.Itam.Application.Commands.Assets.CreateAsset;

public sealed class CreateAssetCommandHandler : ICommandHandler<CreateAssetCommand, AssetDto>
{
    private readonly IAssetRepository _assetRepository;
    private readonly IMapper _mapper;

    public CreateAssetCommandHandler(IAssetRepository assetRepository, IMapper mapper)
    {
        _assetRepository = assetRepository;
        _mapper = mapper;
    }

    public async Task<AssetDto> Handle(CreateAssetCommand request, CancellationToken cancellationToken)
    {
        if (await _assetRepository.ExistsByPatrimonyNumberAsync(request.PatrimonyNumber, cancellationToken: cancellationToken))
            throw new Atlas.Itam.Domain.Errors.ConflictError("An asset with this patrimony number already exists");

        if (await _assetRepository.ExistsBySerialNumberAsync(request.SerialNumber, cancellationToken: cancellationToken))
            throw new Atlas.Itam.Domain.Errors.ConflictError("An asset with this serial number already exists");

        var asset = Asset.Create(
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

        await _assetRepository.AddAsync(asset, cancellationToken);

        return _mapper.Map<AssetDto>(asset);
    }
}
