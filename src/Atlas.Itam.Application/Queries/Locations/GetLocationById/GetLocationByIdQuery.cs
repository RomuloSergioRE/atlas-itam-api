using Atlas.Itam.Application.Common.Interfaces;
using Atlas.Itam.Application.DTOs.Locations;

namespace Atlas.Itam.Application.Queries.Locations.GetLocationById;

public sealed record GetLocationByIdQuery(Guid Id) : IQuery<LocationDto>;
