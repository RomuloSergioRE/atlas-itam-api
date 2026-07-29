using Atlas.Itam.Application.Common.Interfaces;
using Atlas.Itam.Application.DTOs.Locations;
using Atlas.Itam.Domain.Interfaces;
using AutoMapper;

namespace Atlas.Itam.Application.Queries.Locations.GetLocationById;

public sealed class GetLocationByIdQueryHandler : IQueryHandler<GetLocationByIdQuery, LocationDto>
{
    private readonly ILocationRepository _locationRepository;
    private readonly IMapper _mapper;

    public GetLocationByIdQueryHandler(ILocationRepository locationRepository, IMapper mapper)
    {
        _locationRepository = locationRepository;
        _mapper = mapper;
    }

    public async Task<LocationDto> Handle(GetLocationByIdQuery request, CancellationToken cancellationToken)
    {
        var location = await _locationRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new Atlas.Itam.Domain.Errors.NotFoundError("Location not found");

        return _mapper.Map<LocationDto>(location);
    }
}
