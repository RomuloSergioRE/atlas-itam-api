using Atlas.Itam.Application.Common.Interfaces;
using Atlas.Itam.Application.DTOs.Locations;
using Atlas.Itam.Domain.Interfaces;
using AutoMapper;

namespace Atlas.Itam.Application.Queries.Locations.GetLocations;

public sealed class GetLocationsQueryHandler : IQueryHandler<GetLocationsQuery, IReadOnlyList<LocationDto>>
{
    private readonly ILocationRepository _locationRepository;
    private readonly IMapper _mapper;

    public GetLocationsQueryHandler(ILocationRepository locationRepository, IMapper mapper)
    {
        _locationRepository = locationRepository;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<LocationDto>> Handle(GetLocationsQuery request, CancellationToken cancellationToken)
    {
        var locations = await _locationRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<List<LocationDto>>(locations);
    }
}
