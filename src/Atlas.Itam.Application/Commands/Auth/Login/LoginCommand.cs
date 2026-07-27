using Atlas.Itam.Application.Common.Interfaces;
using Atlas.Itam.Application.DTOs.Users;

namespace Atlas.Itam.Application.Commands.Auth.Login;

public sealed record LoginCommand(
    string Email,
    string Password
) : ICommand<AuthDto>;
