namespace Atlas.Itam.Domain.Errors;

public sealed class UnauthorizedError : AppError
{
    public UnauthorizedError()
        : base("Unauthorized", 401)
    {
    }

    public UnauthorizedError(string message)
        : base(message, 401)
    {
    }
}
