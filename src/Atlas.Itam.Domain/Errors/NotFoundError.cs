namespace Atlas.Itam.Domain.Errors;

public sealed class NotFoundError : AppError
{
    public NotFoundError(string resource)
        : base($"{resource} not found", 404)
    {
    }

    public NotFoundError(string resource, Guid id)
        : base($"{resource} with id '{id}' not found", 404)
    {
    }
}
