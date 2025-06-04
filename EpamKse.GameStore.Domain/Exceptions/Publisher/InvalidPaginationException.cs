namespace EpamKse.GameStore.Domain.Exceptions.Publisher;

public class InvalidPaginationException : CustomHttpException
{
    public InvalidPaginationException()
        : base(400, "Page and limit must be greater than zero.")
    {
    }
}