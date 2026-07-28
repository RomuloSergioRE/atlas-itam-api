using Atlas.Itam.Application.Common.Interfaces;
using Atlas.Itam.Application.DTOs.Requests;
using Atlas.Itam.Domain.Entities;
using Atlas.Itam.Domain.Enums;
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
        var pendingCount = await _requestRepository.CountPendingByUserAsync(request.RequestedById, cancellationToken);
        if (pendingCount >= 3)
            throw new Atlas.Itam.Domain.Errors.ConflictError("Maximum of 3 pending requests per user");

        var asset = await _assetRepository.GetByIdAsync(request.AssetId, cancellationToken)
            ?? throw new Atlas.Itam.Domain.Errors.NotFoundError("Asset not found");

        if (asset.CurrentUserId == request.RequestedById)
            throw new Atlas.Itam.Domain.Errors.ConflictError("Cannot request an asset already assigned to you");

        if (asset.Status != AssetStatus.Available)
            throw new Atlas.Itam.Domain.Errors.ConflictError("Asset is not available for request");

        if (await _requestRepository.HasActiveRequestForAssetAsync(request.AssetId, cancellationToken))
            throw new Atlas.Itam.Domain.Errors.ConflictError("Asset already has an active request");

        var requestEntity = Request.Create(request.Justification, request.AssetId, request.RequestedById);
        await _requestRepository.AddAsync(requestEntity, cancellationToken);

        return _mapper.Map<RequestDto>(requestEntity);
    }
}
