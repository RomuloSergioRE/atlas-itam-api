using Atlas.Itam.Application.Common.Interfaces;
using Atlas.Itam.Application.DTOs.Requests;
using Atlas.Itam.Domain.Interfaces;
using AutoMapper;

namespace Atlas.Itam.Application.Queries.Requests.GetRequestById;

public sealed class GetRequestByIdQueryHandler : IQueryHandler<GetRequestByIdQuery, RequestDto>
{
    private readonly IRequestRepository _requestRepository;
    private readonly IMapper _mapper;

    public GetRequestByIdQueryHandler(IRequestRepository requestRepository, IMapper mapper)
    {
        _requestRepository = requestRepository;
        _mapper = mapper;
    }

    public async Task<RequestDto> Handle(GetRequestByIdQuery request, CancellationToken cancellationToken)
    {
        var requestEntity = await _requestRepository.GetByIdAsync(request.RequestId, cancellationToken)
            ?? throw new Atlas.Itam.Domain.Errors.NotFoundError("Request not found");

        return _mapper.Map<RequestDto>(requestEntity);
    }
}
