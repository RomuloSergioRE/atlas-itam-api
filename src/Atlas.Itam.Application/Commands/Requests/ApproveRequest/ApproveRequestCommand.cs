using Atlas.Itam.Application.Common.Interfaces;

namespace Atlas.Itam.Application.Commands.Requests.ApproveRequest;

public sealed record ApproveRequestCommand(
    Guid RequestId,
    Guid ApprovedById
) : ICommand;
