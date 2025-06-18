namespace EpamKse.GameStore.Domain.DTO.GameBan;

using System.ComponentModel.DataAnnotations;
using Enums;

public class CreateGameBanDto {
    [Required(ErrorMessage = "Game ID is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Game ID must be greater than 0")]
    public int GameId { get; set; }
    
    [Required(ErrorMessage = "Country is required")]
    public Countries Country { get; set; }
}
