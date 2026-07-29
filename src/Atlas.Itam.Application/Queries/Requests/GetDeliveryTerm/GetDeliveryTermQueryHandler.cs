using Atlas.Itam.Application.Common.Interfaces;
using Atlas.Itam.Domain.Interfaces;

namespace Atlas.Itam.Application.Queries.Requests.GetDeliveryTerm;

public sealed class GetDeliveryTermQueryHandler : IQueryHandler<GetDeliveryTermQuery, byte[]>
{
    private readonly IRequestRepository _requestRepository;
    private readonly IAssetRepository _assetRepository;
    private readonly IUserRepository _userRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IPdfService _pdfService;

    public GetDeliveryTermQueryHandler(
        IRequestRepository requestRepository,
        IAssetRepository assetRepository,
        IUserRepository userRepository,
        IDepartmentRepository departmentRepository,
        IPdfService pdfService)
    {
        _requestRepository = requestRepository;
        _assetRepository = assetRepository;
        _userRepository = userRepository;
        _departmentRepository = departmentRepository;
        _pdfService = pdfService;
    }

    public async Task<byte[]> Handle(GetDeliveryTermQuery request, CancellationToken cancellationToken)
    {
        var requestEntity = await _requestRepository.GetByIdAsync(request.RequestId, cancellationToken)
            ?? throw new Atlas.Itam.Domain.Errors.NotFoundError("Request not found");

        if (requestEntity.Status != Atlas.Itam.Domain.Enums.RequestStatus.Delivered)
            throw new Atlas.Itam.Domain.Errors.ConflictError("Delivery term is only available for delivered requests");

        var asset = await _assetRepository.GetByIdAsync(requestEntity.AssetId, cancellationToken)
            ?? throw new Atlas.Itam.Domain.Errors.NotFoundError("Asset not found");

        var user = await _userRepository.GetByIdAsync(requestEntity.RequestedById, cancellationToken)
            ?? throw new Atlas.Itam.Domain.Errors.NotFoundError("User not found");

        var department = await _departmentRepository.GetByIdAsync(user.DepartmentId, cancellationToken)
            ?? throw new Atlas.Itam.Domain.Errors.NotFoundError("Department not found");

        var pdf = _pdfService.GenerateDeliveryTerm(
            asset.Name,
            asset.PatrimonyNumber,
            asset.SerialNumber,
            user.Name,
            user.Email,
            department.Name,
            requestEntity.UpdatedAt);

        return pdf;
    }
}
