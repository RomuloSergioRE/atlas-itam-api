using Atlas.Itam.Application.Common.Interfaces;

namespace Atlas.Itam.Application.Commands.Users.DeleteUser;

public sealed record DeleteUserCommand(Guid Id) : ICommand;
