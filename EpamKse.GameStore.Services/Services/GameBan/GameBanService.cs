namespace EpamKse.GameStore.Services.Services.GameBan;

using DataAccess.Repositories.Game;
using DataAccess.Repositories.GameBan;
using Domain.DTO.GameBan;
using Domain.Entities;
using Domain.Exceptions.Game;
using Domain.Exceptions.GameBan;

using Helpers.GameBan;

public class GameBanService(IGameBanRepository banRepository, IGameRepository gameRepository) : IGameBanService {
    public async Task<IEnumerable<GameBanDto>> GetAllBansAsync() {
        var bans = await banRepository.GetAllAsync();
        return bans.Select(b => new GameBanDto {
            Id = b.Id,
            GameId = b.GameId,
            GameTitle = b.Game.Title,
            Country = b.Country,
            CreatedAt = b.CreatedAt
        });
    }

    public async Task<GameBanDto> GetBanByIdAsync(int id) {
        var ban = await banRepository.GetByIdAsync(id);
        if (ban == null) throw new BanNotFoundException(id);

        return new GameBanDto {
            Id = ban.Id,
            GameId = ban.GameId,
            GameTitle = ban.Game.Title,
            Country = ban.Country,
            CreatedAt = ban.CreatedAt
        };
    }

    public async Task<IEnumerable<GameBanDto>> GetBansByCountryAsync(string country) {
        if (!CountryHelper.IsValidCountry(country)) {
            throw new InvalidCountryException(country, CountryHelper.GetValidCountries());
        }
    
        var bans = await banRepository.GetByCountryAsync(CountryHelper.ParseCountry(country));

        return bans.Select(b => new GameBanDto {
            Id = b.Id,
            GameId = b.GameId,
            GameTitle = b.Game.Title,
            Country = b.Country,
            CreatedAt = b.CreatedAt
        });
    }

    public async Task<GameBanDto> CreateBanAsync(CreateGameBanDto dto) {

        if (!CountryHelper.IsValidCountry(dto.Country)) {
            throw new InvalidCountryException(dto.Country, CountryHelper.GetValidCountries());
        }
        
        var country = CountryHelper.ParseCountry(dto.Country);
        
        var game = await gameRepository.GetByIdAsync(dto.GameId);
        if (game == null) throw new GameNotFoundException(dto.GameId);

        var existingBan = await banRepository.GetByGameAndCountryAsync(dto.GameId, country);
        if (existingBan != null) {
            throw new BanAlreadyExistsException(dto.GameId, dto.Country);
        }

        var ban = new GameBan {
            GameId = dto.GameId,
            Country = country,
            CreatedAt = DateTime.UtcNow
        };

        var createdBan = await banRepository.CreateAsync(ban);

        return new GameBanDto {
            Id = createdBan.Id,
            GameId = createdBan.GameId,
            GameTitle = createdBan.Game.Title,
            Country = createdBan.Country,
            CreatedAt = createdBan.CreatedAt
        };
    }

    public async Task DeleteBanAsync(int id) {
        var ban = await banRepository.GetByIdAsync(id);
        if (ban == null) {
            throw new BanNotFoundException(id);
        }

        await banRepository.DeleteAsync(ban);
    }
}
