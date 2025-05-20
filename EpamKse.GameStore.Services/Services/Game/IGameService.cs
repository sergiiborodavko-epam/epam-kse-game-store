namespace EpamKse.GameStore.Services.Services.Game;

using Domain.DTO;
using Domain.Entities;

public interface IGameService {
    Task<IEnumerable<Game>> GetAllGamesAsync();
    Task<Game?> GetGameByIdAsync(int id);
    Task<Game> CreateGameAsync(GameDTO gameDto);
    Task<Game?> UpdateGameAsync(int id, GameDTO gameDto);
    Task<bool> DeleteGameAsync(int id);
}
