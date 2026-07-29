using Atlas.Itam.Application.Common.Interfaces;
using Atlas.Itam.Application.DTOs.Locations;
using Atlas.Itam.Domain.Entities;
using Atlas.Itam.Domain.Interfaces;
using AutoMapper;

namespace Atlas.Itam.Application.Commands.Locations.CreateLocation;

public sealed class CreateLocationCommandHandler : ICommandHandler<CreateLocationCommand, LocationDto>
{
    private readonly ILocationRepository _locationRepository;
    private readonly IMapper _mapper;

    public CreateLocationCommandHandler(ILocationRepository locationRepository, IMapper mapper)
    {
        _locationRepository = locationRepository;
        _mapper = mapper;
    }

    public async Task<LocationDto> Handle(CreateLocationCommand request, CancellationToken cancellationToken)
    {
        var location = Location.Create(request.Name, request.Address);
        await _locationRepository.AddAsync(location, cancellationToken);
        return _mapper.Map<LocationDto>(location);
    }
}
