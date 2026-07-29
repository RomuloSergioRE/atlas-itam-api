using Atlas.Itam.Application.Common.Interfaces;
using Atlas.Itam.Application.DTOs.Locations;

namespace Atlas.Itam.Application.Commands.Locations.CreateLocation;

public sealed record CreateLocationCommand(
    string Name,
    string? Address = null
) : ICommand<LocationDto>;
