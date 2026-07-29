using Atlas.Itam.Application.Common.Interfaces;

namespace Atlas.Itam.Application.Commands.Auth;

public sealed record ForgotPasswordCommand(string Email) : ICommand;
