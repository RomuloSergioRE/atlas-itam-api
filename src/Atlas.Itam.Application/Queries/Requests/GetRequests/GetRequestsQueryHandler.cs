using Atlas.Itam.Application.Common.Interfaces;
using Atlas.Itam.Application.DTOs.Requests;
using Atlas.Itam.Domain.Interfaces;
using AutoMapper;

namespace Atlas.Itam.Application.Queries.Requests.GetRequests;

public sealed class GetRequestsQueryHandler : IQueryHandler<GetRequestsQuery, List<RequestDto>>
{
    private readonly IRequestRepository _requestRepository;
    private readonly IMapper _mapper;

    public GetRequestsQueryHandler(IRequestRepository requestRepository, IMapper mapper)
    {
        _requestRepository = requestRepository;
        _mapper = mapper;
    }

    public async Task<List<RequestDto>> Handle(GetRequestsQuery request, CancellationToken cancellationToken)
    {
        var requests = request.UserId.HasValue
            ? await _requestRepository.GetByRequestedByAsync(request.UserId.Value, cancellationToken)
            : await _requestRepository.GetAllAsync(cancellationToken);

        return _mapper.Map<List<RequestDto>>(requests);
    }
}
