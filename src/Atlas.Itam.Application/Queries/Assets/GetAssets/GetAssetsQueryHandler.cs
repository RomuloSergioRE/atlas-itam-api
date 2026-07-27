using Atlas.Itam.Application.Common.Interfaces;
using Atlas.Itam.Application.Common.Mappings;
using Atlas.Itam.Application.DTOs.Assets;
using Atlas.Itam.Domain.Interfaces;
using AutoMapper;

namespace Atlas.Itam.Application.Queries.Assets.GetAssets;

public sealed class GetAssetsQueryHandler : IQueryHandler<GetAssetsQuery, PagedResult<AssetDto>>
{
    private readonly IAssetRepository _assetRepository;
    private readonly IMapper _mapper;

    public GetAssetsQueryHandler(IAssetRepository assetRepository, IMapper mapper)
    {
        _assetRepository = assetRepository;
        _mapper = mapper;
    }

    public async Task<PagedResult<AssetDto>> Handle(GetAssetsQuery request, CancellationToken cancellationToken)
    {
        var (assets, totalCount) = await _assetRepository.GetAllAsync(
            request.Search, request.Status, request.CategoryId, request.LocationId,
            request.Page, request.PageSize, cancellationToken);

        var assetDtos = _mapper.Map<List<AssetDto>>(assets);

        return new PagedResult<AssetDto>(assetDtos, totalCount, request.Page, request.PageSize);
    }
}
