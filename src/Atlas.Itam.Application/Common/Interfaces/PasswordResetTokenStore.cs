using System.Collections.Concurrent;

namespace Atlas.Itam.Application.Common.Interfaces;

public sealed class PasswordResetTokenStore
{
    private static readonly ConcurrentDictionary<string, (string Email, DateTime ExpiresAt)> _tokens = new();

    public string GenerateToken(string email)
    {
        var token = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");
        _tokens[token] = (email, DateTime.UtcNow.AddHours(1));
        return token;
    }

    public string? ValidateToken(string token)
    {
        if (_tokens.TryGetValue(token, out var entry) && entry.ExpiresAt > DateTime.UtcNow)
        {
            _tokens.TryRemove(token, out _);
            return entry.Email;
        }

        if (_tokens.ContainsKey(token))
            _tokens.TryRemove(token, out _);

        return null;
    }
}
