namespace EpamKse.GameStore.Services.Services.GameBan;

using Domain.DTO.GameBan;
using Domain.Enums;

public interface IGameBanService {
    Task<IEnumerable<GameBanDto>> GetAllBansAsync();
    Task<GameBanDto> GetBanByIdAsync(int id);
    Task<IEnumerable<GameBanDto>> GetBansByCountryAsync(string country);
    Task<GameBanDto> CreateBanAsync(CreateGameBanDto dto);
    Task DeleteBanAsync(int id);
}
