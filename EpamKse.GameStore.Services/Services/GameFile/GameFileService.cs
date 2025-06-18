namespace EpamKse.GameStore.Services.Services.GameFile;

using Domain.DTO.GameFile;
using Domain.Entities;
using Domain.Exceptions.GameFile;
using Domain.Exceptions.Game;
using Domain.Exceptions.Platform;
using DataAccess.Repositories.GameFile;
using DataAccess.Repositories.Game;
using DataAccess.Repositories.Platform;

public class GameFileService(IGameFileRepository gameFileRepository, IGameRepository gameRepository, 
    IPlatformRepository platformRepository) : IGameFileService {
    private readonly string _uploadPath = InitializeUploadPath();
    private const long MaxFileSize = 100 * 1024 * 1024; // 100 MB

    private readonly Dictionary<string, string[]> _allowedExtensions = new() {
        { "windows", [".exe", ".msi"] },
        { "android", [".apk"] },
        { "ios", [".ipa"] },
        { "vr", [".exe"] }
    };

    private static string InitializeUploadPath() {
        var path = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
        Directory.CreateDirectory(path);
        return path;
    }

    public async Task<IEnumerable<GameFile>> GetAllAsync() {
        return await gameFileRepository.GetAllAsync();
    }

    public async Task<GameFile> GetByIdAsync(int id) {
        return await gameFileRepository.GetByIdAsync(id) ?? throw new GameFileNotFoundException(id);
    }

    public async Task<IEnumerable<GameFile>> GetByGameIdAsync(int gameId) {
        return await gameFileRepository.GetByGameIdAsync(gameId);
    }

    public async Task<GameFile> UploadFileAsync(CreateGameFileDto dto) {
        await ValidateGameAndPlatformAsync(dto.GameId, dto.PlatformId);
        await ValidateFileAsync(dto);

        var platform = await platformRepository.GetByIdAsync(dto.PlatformId);
        var filePath = await SaveFileAsync(dto, platform!.Name);

        var gameFile = new GameFile {
            FileName = dto.FileName,
            FilePath = filePath,
            FileExtension = dto.FileExtension,
            FileSize = dto.FileSize,
            GameId = dto.GameId,
            PlatformId = dto.PlatformId
        };

        return await gameFileRepository.CreateAsync(gameFile);
    }

    public async Task<byte[]> DownloadFileAsync(int id) {
        var gameFile = await gameFileRepository.GetByIdAsync(id);
        if (gameFile == null) {
            throw new GameFileNotFoundException(id);
        }

        if (!File.Exists(gameFile.FilePath)) {
            throw new FileUploadException("File not found on disk");
        }

        return await File.ReadAllBytesAsync(gameFile.FilePath);
    }

    public async Task DeleteFileAsync(int id) {
        var gameFile = await gameFileRepository.GetByIdAsync(id);
        if (gameFile == null) {
            throw new GameFileNotFoundException(id);
        }

        if (File.Exists(gameFile.FilePath)) {
            File.Delete(gameFile.FilePath);
        }

        await gameFileRepository.DeleteAsync(gameFile);
    }

    private async Task ValidateGameAndPlatformAsync(int gameId, int platformId) {
        var game = await gameRepository.GetByIdAsync(gameId);
        if (game == null) {
            throw new GameNotFoundException(gameId);
        }

        var platform = await platformRepository.GetByIdAsync(platformId);
        if (platform == null) {
            throw new PlatformNotFoundException(platformId);
        }
    }

    private async Task ValidateFileAsync(CreateGameFileDto dto) {
        if (dto.FileSize > MaxFileSize) {
            throw new FileSizeTooLargeException(MaxFileSize);
        }

        var platform = await platformRepository.GetByIdAsync(dto.PlatformId);
        var platformName = platform!.Name.ToLower();
        if (!_allowedExtensions[platformName].Contains(dto.FileExtension.ToLower())) {
            throw new InvalidFileExtensionException(dto.FileExtension, platformName);
        }
        
        var existingFile = await gameFileRepository.GetByGameAndPlatformAsync(dto.GameId, dto.PlatformId);
        if (existingFile != null) {
            throw new GameFileAlreadyExistsException(dto.GameId, platformName);
        }
    }

    private async Task<string> SaveFileAsync(CreateGameFileDto dto, string platformName) {
        var platformDir = Path.Combine(_uploadPath, platformName.ToLower());
        Directory.CreateDirectory(platformDir);
            
        var fileName = $"{dto.GameId}_{dto.PlatformId}_{dto.FileName}";
        var filePath = Path.Combine(platformDir, fileName);
            
        await File.WriteAllBytesAsync(filePath, dto.FileContent);
        return filePath;
    }
}
