namespace EpamKse.GameStore.Domain.DTO.Genre;

using System.ComponentModel.DataAnnotations;

public class GenreDto {
    [Required(ErrorMessage = "Name is required")]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 50 characters")]
    public string Name { get; set; } = string.Empty;
    
    public string? ParentGenreName { get; set; }
}
