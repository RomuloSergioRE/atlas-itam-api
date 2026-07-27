using Atlas.Itam.Application.Common.Interfaces;

namespace Atlas.Itam.Application.Commands.Auth.Logout;

public sealed class LogoutCommandHandler : ICommandHandler<LogoutCommand>
{
    public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }
}
