namespace EpamKse.GameStore.Services.Services.Game;

using Domain.DTO;
using Domain.Entities;
using Domain.Exceptions;
using DataAccess.Repositories.Game;

public class GameService(IGameRepository gameRepository) : IGameService {
    public async Task<IEnumerable<Game>> GetAllGamesAsync() {
        return await gameRepository.GetAllAsync();
    }

    public async Task<Game> GetGameByIdAsync(int id) {
        var game = await gameRepository.GetByIdAsync(id);
        return game ?? throw new GameNotFoundException(id);
    }

    public async Task<Game> CreateGameAsync(GameDTO gameDto) {
        var existingGame = await gameRepository.GetByTitleAsync(gameDto.Title);
        if (existingGame != null) {
            throw new GameAlreadyExistsException(gameDto.Title);
        }

        var game = new Game {
            Title = gameDto.Title,
            Description = gameDto.Description,
            Price = gameDto.Price,
            ReleaseDate = gameDto.ReleaseDate
        };
        return await gameRepository.CreateAsync(game);
    }

    public async Task<Game> UpdateGameAsync(int id, GameDTO gameDto) {
        var existingGame = await gameRepository.GetByIdAsync(id);
        if (existingGame == null) {
            throw new GameNotFoundException(id);
        }
        
        var gameWithSameTitle = await gameRepository.GetByTitleAsync(gameDto.Title);
        if (gameWithSameTitle != null && gameWithSameTitle.Id != id) {
            throw new GameAlreadyExistsException(gameDto.Title);
        }

        existingGame.Title = gameDto.Title;
        existingGame.Description = gameDto.Description;
        existingGame.Price = gameDto.Price;
        existingGame.ReleaseDate = gameDto.ReleaseDate;
        return await gameRepository.UpdateAsync(existingGame);
    }

    public async Task DeleteGameAsync(int id) {
        var existingGame = await gameRepository.GetByIdAsync(id);
        if (existingGame == null) {
            throw new GameNotFoundException(id);
        }

        await gameRepository.DeleteAsync(existingGame);
    }
}
