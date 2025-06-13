using EpamKse.GameStore.DataAccess.Repositories.HistoricalPrice;
using EpamKse.GameStore.DataAccess.Repositories.Publisher;
using EpamKse.GameStore.Domain.Exceptions.Publisher;

namespace EpamKse.GameStore.Services.Services.Game;

using Domain.DTO.Game;
using Domain.Entities;
using Domain.Exceptions.Game;
using Domain.Exceptions.Genre;
using DataAccess.Repositories.Game;
using DataAccess.Repositories.Genre;

public class GameService(
    IGameRepository gameRepository,
    IGenreRepository genreRepository,
    IHistoricalPriceRepository historicalPriceRepository,
    IPublisherRepository publisherRepository) : IGameService
{
    public async Task<IEnumerable<Game>> GetAllGamesAsync()
    {
        return await gameRepository.GetAllAsync();
    }

    public async Task<Game> GetGameByIdAsync(int id)
    {
        var game = await gameRepository.GetByIdAsync(id);
        return game ?? throw new GameNotFoundException(id);
    }

    public async Task<GameViewDto> CreateGameAsync(GameDto gameDto)
    {
        var existingGame = await gameRepository.GetByTitleAsync(gameDto.Title);
        if (existingGame != null)
        {
            throw new GameAlreadyExistsException(gameDto.Title);
        }

        var publisher = await publisherRepository.GetByIdAsync(gameDto.PublisherId);
        if (publisher == null)
        {
            throw new PublisherNotFoundException(gameDto.PublisherId);
        }

        await ValidateGenresAsync(gameDto.GenreNames, gameDto.SubGenreNames);

        var allGenreNames = gameDto.GenreNames.Concat(gameDto.SubGenreNames).ToList();
        var genres = await genreRepository.GetByNamesAsync(allGenreNames);

        var game = new Game
        {
            Title = gameDto.Title,
            Description = gameDto.Description,
            Price = gameDto.Price,
            ReleaseDate = gameDto.ReleaseDate,
            GenreIds = genres.Select(g => g.Id).ToList(),
            PublisherId = publisher.Id
        };
        var returnGame = await gameRepository.CreateAsync(game);
        await historicalPriceRepository.CreateHistoricalPrice(returnGame.Price, returnGame.Id);
        return new GameViewDto
        {
            Id = returnGame.Id,
            Title = returnGame.Title,
            Description = returnGame.Description,
            Price = returnGame.Price,
            ReleaseDate = returnGame.ReleaseDate,
            GenreIds = returnGame.GenreIds,
        };
    }

    public async Task<GameViewDto> UpdateGameAsync(int id, GameDto gameDto)
    {
        var existingGame = await gameRepository.GetByIdAsync(id);
        if (existingGame == null)
        {
            throw new GameNotFoundException(id);
        }

        var gameWithSameTitle = await gameRepository.GetByTitleAsync(gameDto.Title);
        if (gameWithSameTitle != null && gameWithSameTitle.Id != id)
        {
            throw new GameAlreadyExistsException(gameDto.Title);
        }

        await ValidateGenresAsync(gameDto.GenreNames, gameDto.SubGenreNames);

        var allGenreNames = gameDto.GenreNames.Concat(gameDto.SubGenreNames).ToList();
        var genres = await genreRepository.GetByNamesAsync(allGenreNames);
        if (existingGame.Price != gameDto.Price)
        {
            await historicalPriceRepository.CreateHistoricalPrice(gameDto.Price, id);
        }

        var publisher = await publisherRepository.GetByIdAsync(gameDto.PublisherId);
        if (publisher == null)
        {
            throw new PublisherNotFoundException(gameDto.PublisherId);
        }

        existingGame.Title = gameDto.Title;
        existingGame.Description = gameDto.Description;
        existingGame.Price = gameDto.Price;
        existingGame.ReleaseDate = gameDto.ReleaseDate;
        existingGame.GenreIds = genres.Select(g => g.Id).ToList();
        existingGame.PublisherId = publisher.Id;
        var updatedGame = await gameRepository.UpdateAsync(existingGame);
        return new GameViewDto
        {
            Id = updatedGame.Id,
            Title = updatedGame.Title,
            Description = updatedGame.Description,
            Price = updatedGame.Price,
            ReleaseDate = updatedGame.ReleaseDate,
            GenreIds = updatedGame.GenreIds,
        };
    }

    public async Task DeleteGameAsync(int id)
    {
        var existingGame = await gameRepository.GetByIdAsync(id);
        if (existingGame == null)
        {
            throw new GameNotFoundException(id);
        }

        await gameRepository.DeleteAsync(existingGame);
    }

    public async Task<bool> SetPublisherToGame(SetPublisherDto setPublisherDto)
    {
        var game = await gameRepository.GetByIdAsync(setPublisherDto.gameId);
        var publisher = await publisherRepository.GetByIdAsync(setPublisherDto.publisherId);
        if (game is null)
        {
            throw new GameNotFoundException(setPublisherDto.gameId);
        }

        if (publisher is null)
        {
            throw new PublisherNotFoundException(setPublisherDto.publisherId);
        }

        game.PublisherId = setPublisherDto.publisherId;
        await gameRepository.UpdateAsync(game);
        return true;
    }

    private async Task ValidateGenresAsync(List<string> genreNames, List<string> subGenreNames)
    {
        var allRequestedGenres = genreNames.Concat(subGenreNames).ToList();
        var foundGenres = await genreRepository.GetByNamesAsync(allRequestedGenres);

        if (foundGenres.Count != allRequestedGenres.Count)
        {
            var missingGenres = allRequestedGenres.Except(foundGenres.Select(g => g.Name));
            throw new GenresNotFoundException(missingGenres);
        }

        var mainGenres = foundGenres.Where(g => g.ParentGenreId == null).ToList();
        var subGenres = foundGenres.Where(g => g.ParentGenreId != null).ToList();

        foreach (var subGenre in subGenres)
        {
            var parentGenre = mainGenres.FirstOrDefault(g => g.Id == subGenre.ParentGenreId);
            if (parentGenre == null)
            {
                throw new SubgenreWithoutParentException(subGenre.Name);
            }
        }

        foreach (var subGenreName in subGenreNames)
        {
            var subGenre = foundGenres.FirstOrDefault(g => g.Name == subGenreName);
            if (subGenre?.ParentGenre == null) continue;
            var parentInRequest = genreNames.Contains(subGenre.ParentGenre.Name);
            if (!parentInRequest)
            {
                throw new SubgenreWithoutParentException(subGenreName);
            }
        }
    }
}