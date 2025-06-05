namespace EpamKse.GameStore.DataAccess.Repositories.HistoricalPrice;
using Domain.Entities;
public interface IHistoricalPriceRepository
{
    Task CreateHistoricalPrice(decimal price, int gameId);
}