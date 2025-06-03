namespace EpamKse.GameStore.Domain.Exceptions.Publisher;

public class InvalidPaginationException : CustomHttpException
{
    public InvalidPaginationException(string message)
        : base(400, message)
    {
    }
}