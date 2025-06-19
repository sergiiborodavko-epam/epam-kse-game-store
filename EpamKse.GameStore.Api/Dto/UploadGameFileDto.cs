using System.ComponentModel.DataAnnotations;

namespace EpamKse.GameStore.Api.Dto;

public class UploadGameFileDto {
    [Required(ErrorMessage = "Game ID is required")]
    public int GameId { get; set; }
    
    [Required(ErrorMessage = "Platform ID is required")]
    public int PlatformId { get; set; }
    
    [Required(ErrorMessage = "File is required")]
    public IFormFile File { get; set; } = null!;
}
