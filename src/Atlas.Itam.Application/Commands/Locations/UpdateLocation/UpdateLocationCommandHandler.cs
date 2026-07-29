using Atlas.Itam.Application.Common.Interfaces;
using Atlas.Itam.Application.DTOs.Locations;
using Atlas.Itam.Domain.Interfaces;
using AutoMapper;

namespace Atlas.Itam.Application.Commands.Locations.UpdateLocation;

public sealed class UpdateLocationCommandHandler : ICommandHandler<UpdateLocationCommand, LocationDto>
{
    private readonly ILocationRepository _locationRepository;
    private readonly IMapper _mapper;

    public UpdateLocationCommandHandler(ILocationRepository locationRepository, IMapper mapper)
    {
        _locationRepository = locationRepository;
        _mapper = mapper;
    }

    public async Task<LocationDto> Handle(UpdateLocationCommand request, CancellationToken cancellationToken)
    {
        var location = await _locationRepository.GetByIdAsync(request.LocationId, cancellationToken)
            ?? throw new Atlas.Itam.Domain.Errors.NotFoundError("Location not found");

        location.Update(request.Name, request.Address);
        await _locationRepository.UpdateAsync(location, cancellationToken);

        return _mapper.Map<LocationDto>(location);
    }
}
