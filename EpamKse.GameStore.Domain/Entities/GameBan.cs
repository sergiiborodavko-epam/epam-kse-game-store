namespace EpamKse.GameStore.Domain.Entities;

using Enums;

public class GameBan {
    public int Id { get; set; }
    public int GameId { get; set; }
    public Countries Country  { get; set; }
    public DateTime CreatedAt { get; set; }
    public Game Game { get; set; }
}
