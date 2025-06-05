using EpamKse.GameStore.DataAccess.Repositories.HistoricalPrice;
using EpamKse.GameStore.Domain.DTO.HistoricalPrice;
using EpamKse.GameStore.Domain.Exceptions;


namespace EpamKse.GameStore.Services.Services.HistoricalPrice;

public class HistoricalPriceService : IHistoricalPriceService
{
    private readonly IHistoricalPriceRepository _historicalPriceRepository;

    public HistoricalPriceService(IHistoricalPriceRepository historicalPriceRepository)
    {
        _historicalPriceRepository = historicalPriceRepository;
    }

    public async Task<List<ReturnHistoricalPricesDTO>> GetPricesForGame(int id, int page, int limit)
    {
        if (page < 1 || limit < 1)
        {
            throw new InvalidPaginationException();
        }

        var skip = (page - 1) * limit;
        var publishers = await _historicalPriceRepository.GetPaginatedHistoricalPrices(id, skip, limit);

        return publishers.Select(p => new ReturnHistoricalPricesDTO
        {
            Id = p.Id,
            Price = p.Price,
            GameId = p.GameId,
            CreatedAt = p.CreatedAt
        }).ToList();
    }
}