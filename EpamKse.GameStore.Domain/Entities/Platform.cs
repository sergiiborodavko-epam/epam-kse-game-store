namespace EpamKse.GameStore.Domain.Entities;

public class Platform
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<Game> Games { get; set; } = new List<Game>();
    public ICollection<Publisher> Publishers { get; set; } = new List<Publisher>();
}