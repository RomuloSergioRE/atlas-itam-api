using Atlas.Itam.Application.Common.Interfaces;
using Atlas.Itam.Application.Common.Mappings;
using Atlas.Itam.Application.DTOs.Users;
using Atlas.Itam.Domain.Interfaces;
using AutoMapper;

namespace Atlas.Itam.Application.Queries.Users.GetUsers;

public sealed class GetUsersQueryHandler : IQueryHandler<GetUsersQuery, PagedResult<UserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetUsersQueryHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<PagedResult<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var (users, totalCount) = await _userRepository.GetAllAsync(
            request.Search, request.Page, request.PageSize, cancellationToken);

        var userDtos = _mapper.Map<List<UserDto>>(users);

        return new PagedResult<UserDto>(userDtos, totalCount, request.Page, request.PageSize);
    }
}
