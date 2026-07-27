using Atlas.Itam.Application.Common.Interfaces;
using Atlas.Itam.Application.DTOs.Requests;
using Atlas.Itam.Domain.Entities;
using Atlas.Itam.Domain.Interfaces;
using AutoMapper;

namespace Atlas.Itam.Application.Commands.Requests.CreateRequest;

public sealed class CreateRequestCommandHandler : ICommandHandler<CreateRequestCommand, RequestDto>
{
    private readonly IRequestRepository _requestRepository;
    private readonly IAssetRepository _assetRepository;
    private readonly IMapper _mapper;

    public CreateRequestCommandHandler(
        IRequestRepository requestRepository,
        IAssetRepository assetRepository,
        IMapper mapper)
    {
        _requestRepository = requestRepository;
        _assetRepository = assetRepository;
        _mapper = mapper;
    }

    public async Task<RequestDto> Handle(CreateRequestCommand request, CancellationToken cancellationToken)
    {
        var asset = await _assetRepository.GetByIdAsync(request.AssetId, cancellationToken)
            ?? throw new Atlas.Itam.Domain.Errors.NotFoundError("Asset not found");

        if (await _requestRepository.HasActiveRequestForAssetAsync(request.AssetId, cancellationToken))
            throw new Atlas.Itam.Domain.Errors.ConflictError("Asset already has an active request");

        var requestEntity = Request.Create(request.Justification, request.AssetId, request.RequestedById);
        await _requestRepository.AddAsync(requestEntity, cancellationToken);

        return _mapper.Map<RequestDto>(requestEntity);
    }
}
