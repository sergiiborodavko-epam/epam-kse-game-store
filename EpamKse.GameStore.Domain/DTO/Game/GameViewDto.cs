namespace EpamKse.GameStore.Domain.DTO.Game;

public class GameViewDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public DateTime ReleaseDate { get; set; }
    public List<int> GenreIds { get; set; }
    public int Stock { get; set; }
}