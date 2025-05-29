namespace EpamKse.GameStore.Services.Services.Game;

using Domain.DTO;
using Domain.Entities;

public interface IGameService {
    Task<IEnumerable<Game>> GetAllGamesAsync();
    Task<Game> GetGameByIdAsync(int id);
    Task<Game> CreateGameAsync(GameDto gameDto);
    Task<Game> UpdateGameAsync(int id, GameDto gameDto);
    Task DeleteGameAsync(int id);
}
