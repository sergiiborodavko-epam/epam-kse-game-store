namespace EpamKse.GameStore.Domain.Entities;

public class Genre
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? ParentGenreId { get; set; }
    public Genre? ParentGenre { get; set; }
    public ICollection<Genre> SubGenres { get; set; } = new List<Genre>();
}