using Atlas.Itam.Application.Common.Interfaces;
using Atlas.Itam.Domain.Enums;
using Atlas.Itam.Domain.Interfaces;

namespace Atlas.Itam.Application.Commands.Requests.ApproveRequest;

public sealed class ApproveRequestCommandHandler : ICommandHandler<ApproveRequestCommand>
{
    private readonly IRequestRepository _requestRepository;
    private readonly IUserRepository _userRepository;

    public ApproveRequestCommandHandler(
        IRequestRepository requestRepository,
        IUserRepository userRepository)
    {
        _requestRepository = requestRepository;
        _userRepository = userRepository;
    }

    public async Task Handle(ApproveRequestCommand request, CancellationToken cancellationToken)
    {
        var requestEntity = await _requestRepository.GetByIdAsync(request.RequestId, cancellationToken)
            ?? throw new Atlas.Itam.Domain.Errors.NotFoundError("Request not found");

        var approver = await _userRepository.GetByIdAsync(request.ApprovedById, cancellationToken)
            ?? throw new Atlas.Itam.Domain.Errors.NotFoundError("Approver not found");

        if (approver.Role == UserRole.Manager)
        {
            var requester = await _userRepository.GetByIdAsync(requestEntity.RequestedById, cancellationToken)
                ?? throw new Atlas.Itam.Domain.Errors.NotFoundError("Requester not found");

            if (requester.DepartmentId != approver.DepartmentId)
                throw new Atlas.Itam.Domain.Errors.ForbiddenError("Manager can only approve requests from their own department");
        }

        requestEntity.Approve(request.ApprovedById);
        await _requestRepository.UpdateAsync(requestEntity, cancellationToken);
    }
}
