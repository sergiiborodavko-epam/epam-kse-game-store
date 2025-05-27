namespace EpamKse.GameStore.Domain.Exceptions;

public class CustomHttpException : Exception
{
    public int StatusCode;
    public CustomHttpException(int statusCode, string message) : base(message)
    {
        StatusCode = statusCode;
    }
}