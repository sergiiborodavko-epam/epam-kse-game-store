namespace EpamKse.GameStore.Services.Services.Game;

using Domain.DTO;
using Domain.Entities;
using Domain.DTO.Game;
public interface IGameService {
    Task<IEnumerable<Game>> GetAllGamesAsync();
    Task<Game> GetGameByIdAsync(int id);
    Task<ReturnGameDTO> CreateGameAsync(GameDTO gameDto);
    Task<ReturnGameDTO> UpdateGameAsync(int id, GameDTO gameDto);
    Task DeleteGameAsync(int id);
}
