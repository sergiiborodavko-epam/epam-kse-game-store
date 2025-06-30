namespace EpamKse.GameStore.Domain.DTO.GameBan;

using System.ComponentModel.DataAnnotations;

public class CreateGameBanDto {
    [Required(ErrorMessage = "Game ID is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Game ID must be greater than 0")]
    public int GameId { get; set; }
    
    [Required(ErrorMessage = "Country is required")]
    [MinLength(2, ErrorMessage = "Country must be at least 2 characters")]
    [MaxLength(2, ErrorMessage = "Country must be exactly 2 characters")]
    public string Country { get; set; } = string.Empty;
}
