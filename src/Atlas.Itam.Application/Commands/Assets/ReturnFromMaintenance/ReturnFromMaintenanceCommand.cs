using Atlas.Itam.Application.Common.Interfaces;

namespace Atlas.Itam.Application.Commands.Assets.ReturnFromMaintenance;

public sealed record ReturnFromMaintenanceCommand(
    Guid AssetId,
    Guid ResponsibleId,
    string? Observation = null
) : ICommand;
