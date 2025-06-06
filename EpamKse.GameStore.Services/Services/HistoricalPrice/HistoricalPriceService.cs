using EpamKse.GameStore.DataAccess.Repositories.HistoricalPrice;
using EpamKse.GameStore.Domain.DTO.HistoricalPrice;
using EpamKse.GameStore.Domain.Exceptions;

namespace EpamKse.GameStore.Services.Services.HistoricalPrice;

using Domain.Entities;

public class HistoricalPriceService : IHistoricalPriceService
{
    private readonly IHistoricalPriceRepository _historicalPriceRepository;

    public HistoricalPriceService(IHistoricalPriceRepository historicalPriceRepository)
    {
        _historicalPriceRepository = historicalPriceRepository;
    }

    public async Task<List<ReturnHistoricalPricesDTO>> GetPricesForGame(int id, int? page, int? limit)
    {
        if ((page.HasValue && !limit.HasValue) || (!page.HasValue && limit.HasValue))
        {
            throw new InvalidPaginationException();
        }

        List<HistoricalPrice> prices;

        if (page.HasValue && limit.HasValue)
        {
            if (page < 1 || limit < 1)
            {
                throw new InvalidPaginationException();
            }

            var skip = (page.Value - 1) * limit.Value;
            prices = await _historicalPriceRepository.GetPaginatedHistoricalPrices(id, skip, limit.Value);
        }
        else
        {
            prices = await _historicalPriceRepository.GetAllHistoricalPrices(id);
        }

        return prices.Select(p => new ReturnHistoricalPricesDTO
        {
            Id = p.Id,
            Price = p.Price,
            GameId = p.GameId,
            CreatedAt = p.CreatedAt
        }).ToList();
    }
}