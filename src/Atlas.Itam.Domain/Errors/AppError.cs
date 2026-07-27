namespace Atlas.Itam.Domain.Errors;

public class AppError : Exception
{
    public int StatusCode { get; }

    public AppError(string message, int statusCode = 500)
        : base(message)
    {
        StatusCode = statusCode;
    }

    public AppError(string message, int statusCode, Exception innerException)
        : base(message, innerException)
    {
        StatusCode = statusCode;
    }
}
