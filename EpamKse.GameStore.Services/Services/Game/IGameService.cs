namespace EpamKse.GameStore.Services.Services.Game;

using Domain.DTO.Game;
using Domain.Entities;

public interface IGameService {
    Task<IEnumerable<Game>> GetAllGamesAsync();
    Task<Game> GetGameByIdAsync(int id);
    Task<GameViewDto> CreateGameAsync(GameDto gameDto);
    Task<GameViewDto> UpdateGameAsync(int id, GameDto gameDto);
    Task DeleteGameAsync(int id);
    Task<bool> SetPublisherToGame(SetPublisherDto setPublisherDto);
    Task<int> SetGameStock(SetStockDto setStockDto);
}
