using System.Net;
using System.Text.Json;

namespace Atlas.Itam.Api.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, message) = exception switch
        {
            Atlas.Itam.Domain.Errors.NotFoundError e => (HttpStatusCode.NotFound, e.Message),
            Atlas.Itam.Domain.Errors.UnauthorizedError e => (HttpStatusCode.Unauthorized, e.Message),
            Atlas.Itam.Domain.Errors.ForbiddenError e => (HttpStatusCode.Forbidden, e.Message),
            Atlas.Itam.Domain.Errors.ConflictError e => (HttpStatusCode.Conflict, e.Message),
            Atlas.Itam.Domain.Errors.ValidationError e => (HttpStatusCode.BadRequest, e.Message),
            _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred")
        };

        context.Response.StatusCode = (int)statusCode;

        var response = new { error = message };
        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        await context.Response.WriteAsync(json);
    }
}
