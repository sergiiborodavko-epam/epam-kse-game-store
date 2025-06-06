namespace EpamKse.GameStore.Services.Services.Game;

using Domain.DTO.Game;
using Domain.Entities;

public interface IGameService {
    Task<IEnumerable<Game>> GetAllGamesAsync();
    Task<Game> GetGameByIdAsync(int id);
    Task<ReturnGameDTO> CreateGameAsync(GameDto gameDto);
    Task<ReturnGameDTO> UpdateGameAsync(int id, GameDto gameDto);
    Task DeleteGameAsync(int id);
}
