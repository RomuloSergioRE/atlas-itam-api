using Atlas.Itam.Application.Common.Interfaces;
using Atlas.Itam.Application.DTOs.Users;

namespace Atlas.Itam.Application.Commands.Auth.RefreshToken;

public sealed record RefreshTokenCommand(
    string AccessToken,
    string RefreshToken
) : ICommand<AuthDto>;
