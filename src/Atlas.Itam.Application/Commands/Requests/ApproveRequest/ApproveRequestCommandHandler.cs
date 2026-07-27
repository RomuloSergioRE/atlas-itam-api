using Atlas.Itam.Application.Common.Interfaces;
using Atlas.Itam.Domain.Interfaces;

namespace Atlas.Itam.Application.Commands.Requests.ApproveRequest;

public sealed class ApproveRequestCommandHandler : ICommandHandler<ApproveRequestCommand>
{
    private readonly IRequestRepository _requestRepository;

    public ApproveRequestCommandHandler(IRequestRepository requestRepository)
    {
        _requestRepository = requestRepository;
    }

    public async Task Handle(ApproveRequestCommand request, CancellationToken cancellationToken)
    {
        var requestEntity = await _requestRepository.GetByIdAsync(request.RequestId, cancellationToken)
            ?? throw new Atlas.Itam.Domain.Errors.NotFoundError("Request not found");

        requestEntity.Approve(request.ApprovedById);
        await _requestRepository.UpdateAsync(requestEntity, cancellationToken);
    }
}
