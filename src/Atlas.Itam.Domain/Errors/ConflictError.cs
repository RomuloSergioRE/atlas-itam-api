namespace Atlas.Itam.Domain.Errors;

public sealed class ConflictError : AppError
{
    public ConflictError(string message)
        : base(message, 409)
    {
    }
}
