using Atlas.Itam.Application.Common.Interfaces;

namespace Atlas.Itam.Application.Commands.Requests.DeliverRequest;

public sealed record DeliverRequestCommand(
    Guid RequestId,
    Guid DeliveredById
) : ICommand;
