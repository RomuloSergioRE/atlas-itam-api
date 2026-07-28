using Atlas.Itam.Application.Commands.Auth.Login;
using Atlas.Itam.Application.Common.Interfaces;
using Atlas.Itam.Application.DTOs.Users;
using Atlas.Itam.Domain.Entities;
using Atlas.Itam.Domain.Enums;
using Atlas.Itam.Domain.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Atlas.Itam.Application.Tests;

[TestClass]
public class LoginCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<IJwtTokenService> _jwtTokenServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _jwtTokenServiceMock = new Mock<IJwtTokenService>();
        _mapperMock = new Mock<IMapper>();
        _configurationMock = new Mock<IConfiguration>();
        _handler = new LoginCommandHandler(
            _userRepositoryMock.Object,
            _passwordHasherMock.Object,
            _jwtTokenServiceMock.Object,
            _mapperMock.Object,
            _configurationMock.Object);
    }

    private void SetupConfiguration()
    {
        _configurationMock.Setup(x => x["JwtSettings:AccessTokenExpirationMinutes"]).Returns("15");
        _configurationMock.Setup(x => x["JwtSettings:RefreshTokenExpirationDays"]).Returns("7");
    }

    [TestMethod]
    public async Task Handle_ShouldReturnAuthDto_WhenValidCredentials()
    {
        SetupConfiguration();
        var user = User.Create("Test", "test@email.com", "hash", UserRole.Admin, Guid.NewGuid());
        _userRepositoryMock.Setup(x => x.GetByEmailAsync("test@email.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordHasherMock.Setup(x => x.VerifyPassword("password", "hash"))
            .Returns(true);
        _jwtTokenServiceMock.Setup(x => x.GenerateAccessToken(It.IsAny<User>()))
            .Returns("access_token");
        _jwtTokenServiceMock.Setup(x => x.GenerateRefreshToken())
            .Returns("refresh_token");
        _mapperMock.Setup(x => x.Map<UserDto>(It.IsAny<User>()))
            .Returns(new UserDto { UserId = user.UserId, Name = user.Name });

        var result = await _handler.Handle(new LoginCommand("test@email.com", "password"), CancellationToken.None);

        Assert.AreEqual("access_token", result.AccessToken);
        Assert.AreEqual("refresh_token", result.RefreshToken);
    }

    [TestMethod]
    [ExpectedException(typeof(Atlas.Itam.Domain.Errors.UnauthorizedError))]
    public async Task Handle_ShouldThrow_WhenUserNotFound()
    {
        _userRepositoryMock.Setup(x => x.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        await _handler.Handle(new LoginCommand("wrong@email.com", "password"), CancellationToken.None);
    }

    [TestMethod]
    [ExpectedException(typeof(Atlas.Itam.Domain.Errors.UnauthorizedError))]
    public async Task Handle_ShouldThrow_WhenPasswordInvalid()
    {
        var user = User.Create("Test", "test@email.com", "hash", UserRole.Admin, Guid.NewGuid());
        _userRepositoryMock.Setup(x => x.GetByEmailAsync("test@email.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordHasherMock.Setup(x => x.VerifyPassword("wrong", "hash"))
            .Returns(false);

        await _handler.Handle(new LoginCommand("test@email.com", "wrong"), CancellationToken.None);
    }

    [TestMethod]
    [ExpectedException(typeof(Atlas.Itam.Domain.Errors.UnauthorizedError))]
    public async Task Handle_ShouldThrow_WhenUserInactive()
    {
        var user = User.Create("Test", "test@email.com", "hash", UserRole.Admin, Guid.NewGuid());
        user.Deactivate();
        _userRepositoryMock.Setup(x => x.GetByEmailAsync("test@email.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordHasherMock.Setup(x => x.VerifyPassword("password", "hash"))
            .Returns(true);

        await _handler.Handle(new LoginCommand("test@email.com", "password"), CancellationToken.None);
    }
}
