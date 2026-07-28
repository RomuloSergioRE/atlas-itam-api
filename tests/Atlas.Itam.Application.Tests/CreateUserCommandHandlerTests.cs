using Atlas.Itam.Application.Commands.Users.CreateUser;
using Atlas.Itam.Application.Common.Interfaces;
using Atlas.Itam.Application.DTOs.Users;
using Atlas.Itam.Domain.Entities;
using Atlas.Itam.Domain.Enums;
using Atlas.Itam.Domain.Interfaces;
using AutoMapper;
using Moq;

namespace Atlas.Itam.Application.Tests;

[TestClass]
public class CreateUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CreateUserCommandHandler _handler;

    public CreateUserCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _mapperMock = new Mock<IMapper>();
        _handler = new CreateUserCommandHandler(
            _userRepositoryMock.Object,
            _passwordHasherMock.Object,
            _mapperMock.Object);
    }

    [TestMethod]
    public async Task Handle_ShouldCreateUser_WhenValidCommand()
    {
        var command = new CreateUserCommand("João Silva", "joao@email.com", "password123", "Admin", Guid.NewGuid());

        _userRepositoryMock.Setup(x => x.GetByEmailAsync("joao@email.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        _passwordHasherMock.Setup(x => x.HashPassword("password123"))
            .Returns("hashed_password");

        var userDto = new UserDto { UserId = Guid.NewGuid(), Name = "João Silva" };
        _mapperMock.Setup(x => x.Map<UserDto>(It.IsAny<User>()))
            .Returns(userDto);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.IsNotNull(result);
        Assert.AreEqual("João Silva", result.Name);
        _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(Atlas.Itam.Domain.Errors.ConflictError))]
    public async Task Handle_ShouldThrowConflict_WhenEmailExists()
    {
        var command = new CreateUserCommand("Test", "existing@email.com", "password", "Admin", Guid.NewGuid());

        var existingUser = User.Create("Existing", "existing@email.com", "hash", UserRole.Admin, Guid.NewGuid());
        _userRepositoryMock.Setup(x => x.GetByEmailAsync("existing@email.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);

        await _handler.Handle(command, CancellationToken.None);
    }
}
