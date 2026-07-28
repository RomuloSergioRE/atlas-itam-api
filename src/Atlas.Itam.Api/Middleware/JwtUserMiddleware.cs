using System.Security.Claims;

namespace Atlas.Itam.Api.Middleware;

public sealed class JwtUserMiddleware
{
    private readonly RequestDelegate _next;

    public JwtUserMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId) && Guid.TryParse(userId, out var parsed))
            {
                context.Items["UserId"] = parsed;
            }
        }

        await _next(context);
    }
}
