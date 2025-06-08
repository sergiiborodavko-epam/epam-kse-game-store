namespace EpamKse.GameStore.Domain.DTO.Publisher;

public class PublisherDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string HomePageUrl { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<GameDTO> Games { get; set; }
    public List<PlatformDTO> Platforms { get; set; }
}

public class GameDTO
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public DateTime ReleaseDate { get; set; }
    public List<PlatformDTO> Platforms { get; set; }
}

public class PlatformDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
}