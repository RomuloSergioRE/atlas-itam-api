using Atlas.Itam.Application.Common.Interfaces;
using Atlas.Itam.Application.DTOs.Users;

namespace Atlas.Itam.Application.Commands.Users.UpdateUser;

public sealed record UpdateUserCommand(
    Guid Id,
    string Name,
    string Email,
    string Role,
    Guid DepartmentId,
    bool IsActive
) : ICommand<UserDto>;
