using Atlas.Itam.Application.Common.Interfaces;

namespace Atlas.Itam.Application.Commands.Locations.DeleteLocation;

public sealed record DeleteLocationCommand(Guid Id) : ICommand;
