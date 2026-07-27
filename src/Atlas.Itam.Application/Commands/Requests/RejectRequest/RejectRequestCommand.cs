using Atlas.Itam.Application.Common.Interfaces;

namespace Atlas.Itam.Application.Commands.Requests.RejectRequest;

public sealed record RejectRequestCommand(
    Guid RequestId,
    Guid ApprovedById,
    string Reason
) : ICommand;
