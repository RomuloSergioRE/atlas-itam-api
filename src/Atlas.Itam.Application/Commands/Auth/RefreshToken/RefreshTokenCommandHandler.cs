using Atlas.Itam.Application.DTOs.Users;
using Atlas.Itam.Application.Common.Interfaces;
using Atlas.Itam.Domain.Interfaces;
using AutoMapper;

namespace Atlas.Itam.Application.Commands.Auth.RefreshToken;

public sealed class RefreshTokenCommandHandler : ICommandHandler<RefreshTokenCommand, AuthDto>
{
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public RefreshTokenCommandHandler(
        IJwtTokenService jwtTokenService,
        IUserRepository userRepository,
        IMapper mapper)
    {
        _jwtTokenService = jwtTokenService;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<AuthDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var userId = _jwtTokenService.ValidateAccessToken(request.AccessToken);

        if (userId is null)
            throw new Atlas.Itam.Domain.Errors.UnauthorizedError("Invalid access token");

        var user = await _userRepository.GetByIdAsync(userId.Value, cancellationToken);

        if (user is null || !user.IsActive)
            throw new Atlas.Itam.Domain.Errors.UnauthorizedError("User not found or inactive");

        var newAccessToken = _jwtTokenService.GenerateAccessToken(user);
        var newRefreshToken = _jwtTokenService.GenerateRefreshToken();

        return new AuthDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15),
            User = _mapper.Map<UserDto>(user)
        };
    }
}
