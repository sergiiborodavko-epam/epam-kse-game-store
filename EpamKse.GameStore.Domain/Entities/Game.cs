namespace EpamKse.GameStore.Domain.Entities;

public class Game {
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime ReleaseDate { get; set; }
    
    // Genres
    // Platforms
    // Publisher
}
