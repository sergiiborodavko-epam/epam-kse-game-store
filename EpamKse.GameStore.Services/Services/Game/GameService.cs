namespace EpamKse.GameStore.Services.Services.Game;

using Domain.DTO.Game;
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

        await ValidateGenresAsync(gameDto.GenreNames, gameDto.SubGenreNames);

        var allGenreNames = gameDto.GenreNames.Concat(gameDto.SubGenreNames).ToList();
        var genres = await genreRepository.GetByNamesAsync(allGenreNames);

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

        await ValidateGenresAsync(gameDto.GenreNames, gameDto.SubGenreNames);

        var allGenreNames = gameDto.GenreNames.Concat(gameDto.SubGenreNames).ToList();
        var genres = await genreRepository.GetByNamesAsync(allGenreNames);

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

    private async Task ValidateGenresAsync(List<string> genreNames, List<string> subGenreNames) {
        var allRequestedGenres = genreNames.Concat(subGenreNames).ToList();
        var foundGenres = await genreRepository.GetByNamesAsync(allRequestedGenres);

        if (foundGenres.Count != allRequestedGenres.Count) {
            var missingGenres = allRequestedGenres.Except(foundGenres.Select(g => g.Name));
            throw new GenresNotFoundException(missingGenres);
        }

        var mainGenres = foundGenres.Where(g => g.ParentGenreId == null).ToList();
        var subGenres = foundGenres.Where(g => g.ParentGenreId != null).ToList();

        foreach (var subGenre in subGenres) {
            var parentGenre = mainGenres.FirstOrDefault(g => g.Id == subGenre.ParentGenreId);
            if (parentGenre == null) {
                throw new SubgenreWithoutParentException(subGenre.Name);
            }
        }

        foreach (var subGenreName in subGenreNames) {
            var subGenre = foundGenres.FirstOrDefault(g => g.Name == subGenreName);
            if (subGenre?.ParentGenre == null) continue;
            var parentInRequest = genreNames.Contains(subGenre.ParentGenre.Name);
            if (!parentInRequest) {
                throw new SubgenreWithoutParentException(subGenreName);
            }
        }
    }
}
