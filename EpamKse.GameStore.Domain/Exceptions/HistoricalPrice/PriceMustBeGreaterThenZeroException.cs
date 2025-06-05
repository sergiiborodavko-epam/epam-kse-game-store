namespace EpamKse.GameStore.Domain.Exceptions.HistoricalPrice;

public class PriceMustBeGreaterThenZeroException: CustomHttpException
{
    public PriceMustBeGreaterThenZeroException()
        : base(400, "Price must be greater then zero")
    {
    }
}