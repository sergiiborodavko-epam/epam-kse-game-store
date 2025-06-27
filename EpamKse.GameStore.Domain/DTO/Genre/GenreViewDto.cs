namespace EpamKse.GameStore.Domain.DTO.Genre;

public class GenreViewDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<GenreViewDto> SubGenres { get; set; } = new List<GenreViewDto>();
}