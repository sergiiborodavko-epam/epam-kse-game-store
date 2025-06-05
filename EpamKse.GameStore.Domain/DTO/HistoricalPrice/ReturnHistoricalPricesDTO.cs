namespace EpamKse.GameStore.Domain.DTO.HistoricalPrice;

public class ReturnHistoricalPricesDTO
{
    public int Id { get; set; }
    public int GameId { get; set; }
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; }
}