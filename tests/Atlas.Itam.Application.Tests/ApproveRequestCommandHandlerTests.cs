using Atlas.Itam.Application.Commands.Requests.ApproveRequest;
using Atlas.Itam.Application.Common.Interfaces;
using Atlas.Itam.Domain.Entities;
using Atlas.Itam.Domain.Enums;
using Atlas.Itam.Domain.Interfaces;
using Moq;

namespace Atlas.Itam.Application.Tests;

[TestClass]
public class ApproveRequestCommandHandlerTests
{
    private readonly Mock<IRequestRepository> _requestRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly ApproveRequestCommandHandler _handler;

    public ApproveRequestCommandHandlerTests()
    {
        _requestRepositoryMock = new Mock<IRequestRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _handler = new ApproveRequestCommandHandler(_requestRepositoryMock.Object, _userRepositoryMock.Object);
    }

    [TestMethod]
    public async Task Handle_ShouldApproveRequest_WhenAdmin()
    {
        var requestEntity = Request.Create("Need laptop", Guid.NewGuid(), Guid.NewGuid());
        var admin = User.Create("Admin", "admin@email.com", "hash", UserRole.Admin, Guid.NewGuid());

        _requestRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(requestEntity);
        _userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(admin);

        await _handler.Handle(new ApproveRequestCommand(requestEntity.RequestId, admin.UserId), CancellationToken.None);

        Assert.AreEqual(RequestStatus.Approved, requestEntity.Status);
        _requestRepositoryMock.Verify(x => x.UpdateAsync(requestEntity, It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(Atlas.Itam.Domain.Errors.ForbiddenError))]
    public async Task Handle_ShouldThrow_WhenManagerFromDifferentDepartment()
    {
        var departmentA = Guid.NewGuid();
        var departmentB = Guid.NewGuid();
        var requester = User.Create("Requester", "req@email.com", "hash", UserRole.HR, departmentA);
        var manager = User.Create("Manager", "mgr@email.com", "hash", UserRole.Manager, departmentB);

        var requestEntity = Request.Create("Need laptop", Guid.NewGuid(), requester.UserId);

        _requestRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(requestEntity);
        _userRepositoryMock.Setup(x => x.GetByIdAsync(manager.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(manager);
        _userRepositoryMock.Setup(x => x.GetByIdAsync(requester.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(requester);

        await _handler.Handle(new ApproveRequestCommand(requestEntity.RequestId, manager.UserId), CancellationToken.None);
    }
}
