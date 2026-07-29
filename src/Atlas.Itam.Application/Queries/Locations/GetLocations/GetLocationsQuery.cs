using Atlas.Itam.Application.Common.Interfaces;

namespace Atlas.Itam.Application.Queries.Locations.GetLocations;

public sealed record GetLocationsQuery() : IQuery<IReadOnlyList<Atlas.Itam.Application.DTOs.Locations.LocationDto>>;
