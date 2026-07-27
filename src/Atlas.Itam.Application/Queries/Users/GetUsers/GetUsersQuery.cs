using Atlas.Itam.Application.Common.Interfaces;
using Atlas.Itam.Application.Common.Mappings;
using Atlas.Itam.Application.DTOs.Users;

namespace Atlas.Itam.Application.Queries.Users.GetUsers;

public sealed record GetUsersQuery(
    string? Search,
    int Page = 1,
    int PageSize = 10
) : IQuery<PagedResult<UserDto>>;
