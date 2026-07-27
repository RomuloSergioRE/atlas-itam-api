using Atlas.Itam.Application.Common.Interfaces;
using Atlas.Itam.Application.DTOs.Users;

namespace Atlas.Itam.Application.Queries.Users.GetUserById;

public sealed record GetUserByIdQuery(Guid Id) : IQuery<UserDto>;
