namespace EpamKse.GameStore.Services.Services.HistoricalPrice;

using Domain.DTO.HistoricalPrice;

public interface IHistoricalPriceService
{
    Task<List<ReturnHistoricalPricesDTO>> GetPricesForGame(int id, int page, int limit);
}