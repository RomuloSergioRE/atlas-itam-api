using System.Text.Json.Serialization;

using Atlas.Itam.Application.Common.Interfaces;

namespace Atlas.Itam.Application.Commands.Auth;

public sealed record ResetPasswordCommand(
    string Token,
    string NewPassword
) : ICommand;
