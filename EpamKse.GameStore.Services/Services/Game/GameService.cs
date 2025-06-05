using EpamKse.GameStore.DataAccess.Repositories.HistoricalPrice;


namespace EpamKse.GameStore.Services.Services.Game;
using Domain.DTO.Game;
using Domain.DTO;
using Domain.Entities;
using Domain.Exceptions;
using DataAccess.Repositories.Game;

public class GameService(IGameRepository gameRepository, IHistoricalPriceRepository historicalPriceRepository) : IGameService {
    public async Task<IEnumerable<Game>> GetAllGamesAsync() {
        return await gameRepository.GetAllAsync();
    }

    public async Task<Game> GetGameByIdAsync(int id) {
        var game = await gameRepository.GetByIdAsync(id);
        return game ?? throw new GameNotFoundException(id);
    }

    public async Task<ReturnGameDTO> CreateGameAsync(GameDTO gameDto) {
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
        var returnGame=await gameRepository.CreateAsync(game);
        await historicalPriceRepository.CreateHistoricalPrice(returnGame.Price, returnGame.Id);
      return new ReturnGameDTO {
          Id = returnGame.Id,
          Title = returnGame.Title,
          Description = returnGame.Description,
          Price = returnGame.Price,
          ReleaseDate = returnGame.ReleaseDate
      };
    }

    public async Task<ReturnGameDTO> UpdateGameAsync(int id, GameDTO gameDto) {
        var existingGame = await gameRepository.GetByIdAsync(id);
        if (existingGame == null) {
            throw new GameNotFoundException(id);
        }
        
        var gameWithSameTitle = await gameRepository.GetByTitleAsync(gameDto.Title);
        if (gameWithSameTitle != null && gameWithSameTitle.Id != id) {
            throw new GameAlreadyExistsException(gameDto.Title);
        }

        if (existingGame.Price!=gameDto.Price)
        {
            await historicalPriceRepository.CreateHistoricalPrice(gameDto.Price, id);
        }
        existingGame.Title = gameDto.Title;
        existingGame.Description = gameDto.Description;
        existingGame.Price = gameDto.Price;
        existingGame.ReleaseDate = gameDto.ReleaseDate;
        var updatedGame= await gameRepository.UpdateAsync(existingGame);
        return new ReturnGameDTO
        {
            Id = updatedGame.Id,
            Title = updatedGame.Title,
            Description = updatedGame.Description,
            Price = updatedGame.Price,
            ReleaseDate = updatedGame.ReleaseDate
        };
    }

    public async Task DeleteGameAsync(int id) {
        var existingGame = await gameRepository.GetByIdAsync(id);
        if (existingGame == null) {
            throw new GameNotFoundException(id);
        }

        await gameRepository.DeleteAsync(existingGame);
    }
}
