namespace Atlas.Itam.Domain.Errors;

public sealed class ForbiddenError : AppError
{
    public ForbiddenError()
        : base("Forbidden", 403)
    {
    }

    public ForbiddenError(string message)
        : base(message, 403)
    {
    }
}
