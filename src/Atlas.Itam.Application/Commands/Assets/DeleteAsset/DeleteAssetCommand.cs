using Atlas.Itam.Application.Common.Interfaces;

namespace Atlas.Itam.Application.Commands.Assets.DeleteAsset;

public sealed record DeleteAssetCommand(Guid AssetId) : ICommand;
