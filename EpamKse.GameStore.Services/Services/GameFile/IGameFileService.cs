namespace EpamKse.GameStore.Services.Services.GameFile;

using Domain.Entities;
using Domain.DTO.GameFile;

public interface IGameFileService {
    Task<IEnumerable<GameFile>> GetAllAsync();
    Task<GameFile> GetByIdAsync(int id);
    Task<IEnumerable<GameFile>> GetByGameIdAsync(int gameId);
    Task<GameFile> UploadFileAsync(CreateGameFileDto dto);
    Task<byte[]> DownloadFileAsync(int id);
    Task DeleteFileAsync(int id);
}
