using Atlas.Itam.Application.Commands.Auth.RefreshToken;
using Atlas.Itam.Application.Common.Interfaces;
using Atlas.Itam.Application.DTOs.Users;
using Atlas.Itam.Domain.Entities;
using Atlas.Itam.Domain.Enums;
using Atlas.Itam.Domain.Interfaces;
using AutoMapper;
using Moq;

namespace Atlas.Itam.Application.Tests;

[TestClass]
public class RefreshTokenCommandHandlerTests
{
    private readonly Mock<IJwtTokenService> _jwtTokenServiceMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly RefreshTokenCommandHandler _handler;

    public RefreshTokenCommandHandlerTests()
    {
        _jwtTokenServiceMock = new Mock<IJwtTokenService>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new RefreshTokenCommandHandler(
            _jwtTokenServiceMock.Object,
            _userRepositoryMock.Object,
            _mapperMock.Object);
    }

    [TestMethod]
    public async Task Handle_ShouldReturnNewTokens_WhenValid()
    {
        var userId = Guid.NewGuid();
        var user = User.Create("User", "user@email.com", "hash", UserRole.Employee, Guid.NewGuid());
        var userDto = new UserDto { UserId = userId, Name = "User" };

        _jwtTokenServiceMock.Setup(x => x.ValidateAccessToken(It.IsAny<string>()))
            .Returns(userId);
        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _jwtTokenServiceMock.Setup(x => x.GenerateAccessToken(It.IsAny<User>()))
            .Returns("new_access_token");
        _jwtTokenServiceMock.Setup(x => x.GenerateRefreshToken())
            .Returns("new_refresh_token");
        _mapperMock.Setup(x => x.Map<UserDto>(It.IsAny<User>()))
            .Returns(userDto);

        var result = await _handler.Handle(
            new RefreshTokenCommand("old_token", "old_refresh"), CancellationToken.None);

        Assert.AreEqual("new_access_token", result.AccessToken);
        Assert.AreEqual("new_refresh_token", result.RefreshToken);
    }

    [TestMethod]
    [ExpectedException(typeof(Atlas.Itam.Domain.Errors.UnauthorizedError))]
    public async Task Handle_ShouldThrow_WhenAccessTokenInvalid()
    {
        _jwtTokenServiceMock.Setup(x => x.ValidateAccessToken(It.IsAny<string>()))
            .Returns((Guid?)null);

        await _handler.Handle(
            new RefreshTokenCommand("invalid", "refresh"), CancellationToken.None);
    }

    [TestMethod]
    [ExpectedException(typeof(Atlas.Itam.Domain.Errors.UnauthorizedError))]
    public async Task Handle_ShouldThrow_WhenUserNotFound()
    {
        var userId = Guid.NewGuid();

        _jwtTokenServiceMock.Setup(x => x.ValidateAccessToken(It.IsAny<string>()))
            .Returns(userId);
        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        await _handler.Handle(
            new RefreshTokenCommand("token", "refresh"), CancellationToken.None);
    }

    [TestMethod]
    [ExpectedException(typeof(Atlas.Itam.Domain.Errors.UnauthorizedError))]
    public async Task Handle_ShouldThrow_WhenUserInactive()
    {
        var userId = Guid.NewGuid();
        var user = User.Create("User", "user@email.com", "hash", UserRole.Employee, Guid.NewGuid());
        user.Deactivate();

        _jwtTokenServiceMock.Setup(x => x.ValidateAccessToken(It.IsAny<string>()))
            .Returns(userId);
        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        await _handler.Handle(
            new RefreshTokenCommand("token", "refresh"), CancellationToken.None);
    }
}
