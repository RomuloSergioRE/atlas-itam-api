using Atlas.Itam.Application.Common.Interfaces;
using Atlas.Itam.Application.DTOs.Users;
using Atlas.Itam.Domain.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Configuration;

namespace Atlas.Itam.Application.Commands.Auth.Login;

public sealed class LoginCommandHandler : ICommandHandler<LoginCommand, AuthDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService,
        IMapper mapper,
        IConfiguration configuration)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _mapper = mapper;
        _configuration = configuration;
    }

    public async Task<AuthDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user is null || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
            throw new Atlas.Itam.Domain.Errors.UnauthorizedError("Invalid credentials");

        if (!user.IsActive)
            throw new Atlas.Itam.Domain.Errors.UnauthorizedError("Account is deactivated");

        var accessToken = _jwtTokenService.GenerateAccessToken(user);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();

        var refreshExpirationDays = int.Parse(_configuration["JwtSettings:RefreshTokenExpirationDays"] ?? "7");

        return new AuthDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(int.Parse(_configuration["JwtSettings:AccessTokenExpirationMinutes"] ?? "15")),
            User = _mapper.Map<UserDto>(user)
        };
    }
}
