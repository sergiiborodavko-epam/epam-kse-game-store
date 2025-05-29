namespace EpamKse.GameStore.Domain.DTO;

using System.ComponentModel.DataAnnotations;

public class GameDto {
    [Required(ErrorMessage = "Title is required")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 100 characters")]
    public string Title { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Description is required")]
    [StringLength(2000, MinimumLength = 1, ErrorMessage = "Description must be between 1 and 2000 characters")]
    public string Description { get; set; } = string.Empty;
    
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }
    
    [Required(ErrorMessage = "Release date is required")]
    public DateTime ReleaseDate { get; set; }
    
    [Required(ErrorMessage = "At least one genre is required")]
    public List<string> GenreNames { get; set; } = [];
}
