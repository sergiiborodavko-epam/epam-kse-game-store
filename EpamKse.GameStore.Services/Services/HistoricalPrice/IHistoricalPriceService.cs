using EpamKse.GameStore.Domain.DTO.HistoricalPrice;

namespace EpamKse.GameStore.Services.Services.HistoricalPrice;

public interface IHistoricalPriceService
{
    Task<List<ReturnHistoricalPricesDTO>> GetPricesForGame(int id, int page, int limit);
}