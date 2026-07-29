using Atlas.Itam.Application.Common.Interfaces;
using Atlas.Itam.Application.DTOs.Locations;

namespace Atlas.Itam.Application.Commands.Locations.UpdateLocation;

public sealed record UpdateLocationCommand(
    Guid LocationId,
    string Name,
    string? Address = null
) : ICommand<LocationDto>;
