namespace EpamKse.GameStore.Services.Services.Game;

using Domain.DTO;
using Domain.Entities;
using DataAccess.Repositories.Game;

public class GameService(IGameRepository gameRepository) : IGameService {
    public async Task<IEnumerable<Game>> GetAllGamesAsync() {
        return await gameRepository.GetAllAsync();
    }

    public async Task<Game?> GetGameByIdAsync(int id) {
        return await gameRepository.GetByIdAsync(id);
    }

    public async Task<Game> CreateGameAsync(GameDTO gameDto) {
        var game = new Game {
            Title = gameDto.Title,
            Description = gameDto.Description,
            Price = gameDto.Price,
            ReleaseDate = gameDto.ReleaseDate
        };
        return await gameRepository.CreateAsync(game);
    }

    public async Task<Game?> UpdateGameAsync(int id, GameDTO gameDto) {
        var existingGame = await gameRepository.GetByIdAsync(id);
        if (existingGame == null) {
            return null;
        }

        existingGame.Title = gameDto.Title;
        existingGame.Description = gameDto.Description;
        existingGame.Price = gameDto.Price;
        existingGame.ReleaseDate = gameDto.ReleaseDate;
        return await gameRepository.UpdateAsync(existingGame);
    }

    public async Task<bool> DeleteGameAsync(int id) {
        var existingGame = await gameRepository.GetByIdAsync(id);
        if (existingGame == null) {
            return false;
        }

        await gameRepository.DeleteAsync(existingGame);
        return true;
    }
}
