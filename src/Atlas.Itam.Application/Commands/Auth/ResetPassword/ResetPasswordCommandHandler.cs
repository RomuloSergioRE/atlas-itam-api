using Atlas.Itam.Application.Common.Interfaces;
using Atlas.Itam.Domain.Interfaces;

namespace Atlas.Itam.Application.Commands.Auth.ResetPassword;

public sealed class ResetPasswordCommandHandler : ICommandHandler<ResetPasswordCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly PasswordResetTokenStore _tokenStore;

    public ResetPasswordCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        PasswordResetTokenStore tokenStore)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenStore = tokenStore;
    }

    public async Task Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var email = _tokenStore.ValidateToken(request.Token);
        if (email is null)
            throw new Atlas.Itam.Domain.Errors.UnauthorizedError("Invalid or expired reset token");

        var user = await _userRepository.GetByEmailAsync(email, cancellationToken)
            ?? throw new Atlas.Itam.Domain.Errors.NotFoundError("User not found");

        var newHash = _passwordHasher.HashPassword(request.NewPassword);
        user.SetPassword(newHash);

        await _userRepository.UpdateAsync(user, cancellationToken);
    }
}
