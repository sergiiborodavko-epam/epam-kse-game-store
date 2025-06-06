namespace EpamKse.GameStore.Domain.Exceptions;

public class InvalidPaginationException : CustomHttpException
{
    public InvalidPaginationException()
        : base(400, "Page and limit must be present and greater than zero.")
    {
    }
}