using Atlas.Itam.Application.Common.Interfaces;
using Atlas.Itam.Domain.Interfaces;

namespace Atlas.Itam.Application.Commands.Locations.DeleteLocation;

public sealed class DeleteLocationCommandHandler : ICommandHandler<DeleteLocationCommand>
{
    private readonly ILocationRepository _locationRepository;

    public DeleteLocationCommandHandler(ILocationRepository locationRepository)
    {
        _locationRepository = locationRepository;
    }

    public async Task Handle(DeleteLocationCommand request, CancellationToken cancellationToken)
    {
        var location = await _locationRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new Atlas.Itam.Domain.Errors.NotFoundError("Location not found");

        await _locationRepository.DeleteAsync(location, cancellationToken);
    }
}
