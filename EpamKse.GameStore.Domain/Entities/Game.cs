﻿namespace EpamKse.GameStore.Domain.Entities;

public class Game {
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime ReleaseDate { get; set; }
    public List<int> GenreIds { get; set; } = [];

    public int? PublisherId { get; set; }
    public Publisher Publisher { get; set; }
    public ICollection<Platform> Platforms { get; set; } = new List<Platform>();
    public int Stock { get; set; }
    public ICollection<HistoricalPrice> HistoricalPrices { get; set; } = new List<HistoricalPrice>();
    public ICollection<GameFile> GameFiles { get; set; } = new List<GameFile>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<GameBan> GameBans { get; set; } = new List<GameBan>();
}
