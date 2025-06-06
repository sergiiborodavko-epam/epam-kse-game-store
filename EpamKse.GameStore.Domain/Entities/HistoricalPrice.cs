namespace EpamKse.GameStore.Domain.Entities;

public class HistoricalPrice
{
    public int Id { get; set; }
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; } 
    public int GameId { get; set; }
    public Game Game { get; set; }
}