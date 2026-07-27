using Atlas.Itam.Application.Common.Interfaces;
using Atlas.Itam.Application.DTOs.Assets;

namespace Atlas.Itam.Application.Commands.Assets.UpdateAsset;

public sealed record UpdateAssetCommand(
    Guid AssetId,
    string Name,
    string PatrimonyNumber,
    string SerialNumber,
    DateTime AcquisitionDate,
    decimal AcquisitionValue,
    Guid CategoryId,
    Guid LocationId,
    string? Supplier = null,
    DateTime? WarrantyUntil = null,
    string? Observations = null
) : ICommand<AssetDto>;
