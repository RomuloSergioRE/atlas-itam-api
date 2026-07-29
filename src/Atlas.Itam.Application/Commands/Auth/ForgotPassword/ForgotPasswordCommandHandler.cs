using Atlas.Itam.Application.Common.Interfaces;
using Atlas.Itam.Domain.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Atlas.Itam.Application.Commands.Auth.ForgotPassword;

public sealed class ForgotPasswordCommandHandler : ICommandHandler<ForgotPasswordCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;
    private readonly PasswordResetTokenStore _tokenStore;
    private readonly IConfiguration _configuration;

    public ForgotPasswordCommandHandler(
        IUserRepository userRepository,
        IEmailService emailService,
        PasswordResetTokenStore tokenStore,
        IConfiguration configuration)
    {
        _userRepository = userRepository;
        _emailService = emailService;
        _tokenStore = tokenStore;
        _configuration = configuration;
    }

    public async Task Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user is null) return;

        var token = _tokenStore.GenerateToken(request.Email);
        var baseUrl = _configuration["AppSettings:BaseUrl"] ?? "http://localhost:5000";
        var resetLink = $"{baseUrl}/reset-password?token={token}";

        await _emailService.SendAsync(
            request.Email,
            "Atlas ITAM - Recuperação de Senha",
            $"""
            <h2>Recuperação de Senha</h2>
            <p>Olá {user.Name},</p>
            <p>Recebemos uma solicitação para redefinir sua senha. Clique no link abaixo:</p>
            <p><a href="{resetLink}">Redefinir Senha</a></p>
            <p>Este link expira em 1 hora.</p>
            <p>Se você não solicitou esta alteração, ignore este e-mail.</p>
            """,
            cancellationToken);
    }
}
