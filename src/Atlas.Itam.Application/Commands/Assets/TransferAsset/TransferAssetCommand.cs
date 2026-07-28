using Atlas.Itam.Application.Common.Interfaces;

namespace Atlas.Itam.Application.Commands.Assets.TransferAsset;

public sealed record TransferAssetCommand(
    Guid AssetId,
    Guid FromUserId,
    Guid ToUserId,
    Guid ResponsibleId,
    string? Observation = null
) : ICommand;
