using Atlas.Itam.Application.Common.Interfaces;
using Atlas.Itam.Domain.Interfaces;

namespace Atlas.Itam.Application.Commands.Requests.RejectRequest;

public sealed class RejectRequestCommandHandler : ICommandHandler<RejectRequestCommand>
{
    private readonly IRequestRepository _requestRepository;

    public RejectRequestCommandHandler(IRequestRepository requestRepository)
    {
        _requestRepository = requestRepository;
    }

    public async Task Handle(RejectRequestCommand request, CancellationToken cancellationToken)
    {
        var requestEntity = await _requestRepository.GetByIdAsync(request.RequestId, cancellationToken)
            ?? throw new Atlas.Itam.Domain.Errors.NotFoundError("Request not found");

        requestEntity.Reject(request.ApprovedById, request.Reason);
        await _requestRepository.UpdateAsync(requestEntity, cancellationToken);
    }
}
