using Atlas.Itam.Application.Commands.Auth.ForgotPassword;
using Atlas.Itam.Application.Common.Interfaces;
using Atlas.Itam.Domain.Entities;
using Atlas.Itam.Domain.Enums;
using Atlas.Itam.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Atlas.Itam.Application.Tests;

[TestClass]
public class ForgotPasswordCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly PasswordResetTokenStore _tokenStore;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly ForgotPasswordCommandHandler _handler;

    public ForgotPasswordCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _emailServiceMock = new Mock<IEmailService>();
        _tokenStore = new PasswordResetTokenStore();
        _configurationMock = new Mock<IConfiguration>();
        _handler = new ForgotPasswordCommandHandler(
            _userRepositoryMock.Object,
            _emailServiceMock.Object,
            _tokenStore,
            _configurationMock.Object);
    }

    [TestMethod]
    public async Task Handle_ShouldSendEmail_WhenUserExists()
    {
        var user = User.Create("User", "user@email.com", "hash", UserRole.Employee, Guid.NewGuid());

        _userRepositoryMock.Setup(x => x.GetByEmailAsync("user@email.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _configurationMock.Setup(x => x["AppSettings:BaseUrl"])
            .Returns("http://localhost:5000");

        await _handler.Handle(new ForgotPasswordCommand("user@email.com"), CancellationToken.None);

        _emailServiceMock.Verify(x => x.SendAsync(
            "user@email.com",
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod]
    public async Task Handle_ShouldNotSendEmail_WhenUserNotFound()
    {
        _userRepositoryMock.Setup(x => x.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        await _handler.Handle(new ForgotPasswordCommand("unknown@email.com"), CancellationToken.None);

        _emailServiceMock.Verify(x => x.SendAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
