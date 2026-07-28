using Atlas.Itam.Application.Common.Interfaces;
using Atlas.Itam.Application.DTOs.Requests;
using Atlas.Itam.Domain.Enums;
using Atlas.Itam.Domain.Interfaces;
using AutoMapper;

namespace Atlas.Itam.Application.Queries.Requests.ListPendingApprovals;

public sealed class ListPendingApprovalsQueryHandler : IQueryHandler<ListPendingApprovalsQuery, List<RequestDto>>
{
    private readonly IRequestRepository _requestRepository;
    private readonly IMapper _mapper;

    public ListPendingApprovalsQueryHandler(IRequestRepository requestRepository, IMapper mapper)
    {
        _requestRepository = requestRepository;
        _mapper = mapper;
    }

    public async Task<List<RequestDto>> Handle(ListPendingApprovalsQuery request, CancellationToken cancellationToken)
    {
        var pendingRequests = await _requestRepository.GetByStatusAsync(RequestStatus.Pending, cancellationToken);
        return _mapper.Map<List<RequestDto>>(pendingRequests);
    }
}
