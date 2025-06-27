namespace EpamKse.GameStore.Domain.DTO.GameBan;

using Enums;

public class GameBanDto {
    public int Id { get; set; }
    public int GameId { get; set; }
    public string GameTitle { get; set; }
    public Countries Country { get; set; }
    public DateTime CreatedAt { get; set; }
}
