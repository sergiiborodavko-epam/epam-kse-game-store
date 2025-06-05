namespace EpamKse.GameStore.Domain.DTO.Game;


public class ReturnGameDTO {
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public DateTime ReleaseDate { get; set; }
}

