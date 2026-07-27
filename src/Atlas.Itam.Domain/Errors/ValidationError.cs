namespace Atlas.Itam.Domain.Errors;

public sealed class ValidationError : AppError
{
    public IReadOnlyList<string> Errors { get; }

    public ValidationError(string message)
        : base(message, 400)
    {
        Errors = new[] { message };
    }

    public ValidationError(IEnumerable<string> errors)
        : base("Validation failed", 400)
    {
        Errors = errors.ToList().AsReadOnly();
    }
}
