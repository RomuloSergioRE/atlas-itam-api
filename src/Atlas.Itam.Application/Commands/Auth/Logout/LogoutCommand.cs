using Atlas.Itam.Application.Common.Interfaces;

namespace Atlas.Itam.Application.Commands.Auth.Logout;

public sealed record LogoutCommand(
    Guid UserId
) : ICommand;
