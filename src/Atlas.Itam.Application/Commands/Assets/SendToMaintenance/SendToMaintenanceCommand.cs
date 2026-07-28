using Atlas.Itam.Application.Common.Interfaces;

namespace Atlas.Itam.Application.Commands.Assets.SendToMaintenance;

public sealed record SendToMaintenanceCommand(
    Guid AssetId,
    Guid ResponsibleId,
    string? Observation = null
) : ICommand;
