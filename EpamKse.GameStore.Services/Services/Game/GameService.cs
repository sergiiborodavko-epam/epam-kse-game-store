namespace EpamKse.GameStore.Services.Services.Game;

using Domain.DTO;
using Domain.Entities;
using Domain.Exceptions.Game;
using Domain.Exceptions.Genre;
using DataAccess.Repositories.Game;
using DataAccess.Repositories.Genre;

public class GameService(IGameRepository gameRepository, IGenreRepository genreRepository) : IGameService {
    public async Task<IEnumerable<Game>> GetAllGamesAsync() {
        return await gameRepository.GetAllAsync();
    }

    public async Task<Game> GetGameByIdAsync(int id) {
        var game = await gameRepository.GetByIdAsync(id);
        return game ?? throw new GameNotFoundException(id);
    }

    public async Task<Game> CreateGameAsync(GameDto gameDto) {
        var existingGame = await gameRepository.GetByTitleAsync(gameDto.Title);
        if (existingGame != null) {
            throw new GameAlreadyExistsException(gameDto.Title);
        }
        
        var genres = await genreRepository.GetByNamesAsync(gameDto.GenreNames);
        if (genres.Count != gameDto.GenreNames.Count) {
            var missingGenres = gameDto.GenreNames.Except(genres.Select(g => g.Name));
            throw new GenreNamesNotFoundException(missingGenres);
        }

        var game = new Game {
            Title = gameDto.Title,
            Description = gameDto.Description,
            Price = gameDto.Price,
            ReleaseDate = gameDto.ReleaseDate,
            GenreIds = genres.Select(g => g.Id).ToList()
        };
        return await gameRepository.CreateAsync(game);
    }

    public async Task<Game> UpdateGameAsync(int id, GameDto gameDto) {
        var existingGame = await gameRepository.GetByIdAsync(id);
        if (existingGame == null) {
            throw new GameNotFoundException(id);
        }
        
        var gameWithSameTitle = await gameRepository.GetByTitleAsync(gameDto.Title);
        if (gameWithSameTitle != null && gameWithSameTitle.Id != id) {
            throw new GameAlreadyExistsException(gameDto.Title);
        }

        var genres = await genreRepository.GetByNamesAsync(gameDto.GenreNames);
        if (genres.Count != gameDto.GenreNames.Count) {
            var missingGenres = gameDto.GenreNames.Except(genres.Select(g => g.Name));
            throw new GenreNamesNotFoundException(missingGenres);
        }

        existingGame.Title = gameDto.Title;
        existingGame.Description = gameDto.Description;
        existingGame.Price = gameDto.Price;
        existingGame.ReleaseDate = gameDto.ReleaseDate;
        existingGame.GenreIds = genres.Select(g => g.Id).ToList();
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
