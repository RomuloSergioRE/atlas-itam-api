using Atlas.Itam.Application.Common.Interfaces;

namespace Atlas.Itam.Application.Commands.Assets.RetireAsset;

public sealed record RetireAssetCommand(
    Guid AssetId,
    Guid ResponsibleId,
    string? Observation = null
) : ICommand;
