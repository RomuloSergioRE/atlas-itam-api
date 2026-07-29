using Atlas.Itam.Application.Commands.Auth.ResetPassword;
using Atlas.Itam.Application.Common.Interfaces;
using Atlas.Itam.Domain.Entities;
using Atlas.Itam.Domain.Enums;
using Atlas.Itam.Domain.Interfaces;
using Moq;

namespace Atlas.Itam.Application.Tests;

[TestClass]
public class ResetPasswordCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly PasswordResetTokenStore _tokenStore;
    private readonly ResetPasswordCommandHandler _handler;

    public ResetPasswordCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _tokenStore = new PasswordResetTokenStore();
        _handler = new ResetPasswordCommandHandler(
            _userRepositoryMock.Object,
            _passwordHasherMock.Object,
            _tokenStore);
    }

    [TestMethod]
    public async Task Handle_ShouldResetPassword_WhenTokenValid()
    {
        var email = "user@email.com";
        var token = _tokenStore.GenerateToken(email);
        var user = User.Create("User", email, "old_hash", UserRole.Employee, Guid.NewGuid());

        _userRepositoryMock.Setup(x => x.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordHasherMock.Setup(x => x.HashPassword("new_password"))
            .Returns("new_hash");

        await _handler.Handle(new ResetPasswordCommand(token, "new_password"), CancellationToken.None);

        _userRepositoryMock.Verify(x => x.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(Atlas.Itam.Domain.Errors.UnauthorizedError))]
    public async Task Handle_ShouldThrow_WhenTokenInvalid()
    {
        await _handler.Handle(new ResetPasswordCommand("invalid_token", "new_password"), CancellationToken.None);
    }

    [TestMethod]
    [ExpectedException(typeof(Atlas.Itam.Domain.Errors.NotFoundError))]
    public async Task Handle_ShouldThrow_WhenUserNotFound()
    {
        var token = _tokenStore.GenerateToken("unknown@email.com");

        _userRepositoryMock.Setup(x => x.GetByEmailAsync("unknown@email.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        await _handler.Handle(new ResetPasswordCommand(token, "new_password"), CancellationToken.None);
    }
}
