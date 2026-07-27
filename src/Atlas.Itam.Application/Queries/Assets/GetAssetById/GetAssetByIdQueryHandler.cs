using Atlas.Itam.Application.Common.Interfaces;
using Atlas.Itam.Application.DTOs.Assets;
using Atlas.Itam.Domain.Interfaces;
using AutoMapper;

namespace Atlas.Itam.Application.Queries.Assets.GetAssetById;

public sealed class GetAssetByIdQueryHandler : IQueryHandler<GetAssetByIdQuery, AssetDto>
{
    private readonly IAssetRepository _assetRepository;
    private readonly IMapper _mapper;

    public GetAssetByIdQueryHandler(IAssetRepository assetRepository, IMapper mapper)
    {
        _assetRepository = assetRepository;
        _mapper = mapper;
    }

    public async Task<AssetDto> Handle(GetAssetByIdQuery request, CancellationToken cancellationToken)
    {
        var asset = await _assetRepository.GetByIdAsync(request.AssetId, cancellationToken)
            ?? throw new Atlas.Itam.Domain.Errors.NotFoundError("Asset not found");

        return _mapper.Map<AssetDto>(asset);
    }
}
