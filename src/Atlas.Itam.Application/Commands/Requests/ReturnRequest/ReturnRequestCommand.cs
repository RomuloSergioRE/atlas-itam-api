using Atlas.Itam.Application.Common.Interfaces;

namespace Atlas.Itam.Application.Commands.Requests.ReturnRequest;

public sealed record ReturnRequestCommand(
    Guid RequestId,
    Guid ReturnedById,
    string? Observation = null
) : ICommand;
