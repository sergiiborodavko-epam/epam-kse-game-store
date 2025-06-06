namespace EpamKse.GameStore.Domain.Exceptions.HistoricalPrice;

public class InvalidPriceException: CustomHttpException
{
    public InvalidPriceException()
        : base(400, "Price must be greater then zero")
    {
    }
}