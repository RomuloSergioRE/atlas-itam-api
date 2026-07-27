using Atlas.Itam.Application.Common.Interfaces;
using Atlas.Itam.Application.DTOs.Users;

namespace Atlas.Itam.Application.Commands.Users.CreateUser;

public sealed record CreateUserCommand(
    string Name,
    string Email,
    string Password,
    string Role,
    Guid DepartmentId
) : ICommand<UserDto>;
