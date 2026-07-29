using Atlas.Itam.Application.Commands.Auth.Logout;

namespace Atlas.Itam.Application.Tests;

[TestClass]
public class LogoutCommandHandlerTests
{
    [TestMethod]
    public async Task Handle_ShouldComplete_WithoutError()
    {
        var handler = new LogoutCommandHandler();

        await handler.Handle(new LogoutCommand(Guid.NewGuid()), CancellationToken.None);
    }
}
