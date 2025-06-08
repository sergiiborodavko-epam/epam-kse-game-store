namespace EpamKse.GameStore.Domain.Entities;

public class Publisher
{
    public int Id { set; get; }
    public string Name { get; set; }
    public string HomePageUrl { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public ICollection<Game> Games { get; set; } = new List<Game>();
    public ICollection<Platform> PublisherPlatforms { get; set; } = new List<Platform>();
}
