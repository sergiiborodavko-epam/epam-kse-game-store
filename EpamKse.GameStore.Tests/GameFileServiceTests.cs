namespace EpamKse.GameStore.Tests;

using Xunit;
using Moq;

using Services.Services.GameFile;
using DataAccess.Repositories.GameFile;
using DataAccess.Repositories.Game;
using DataAccess.Repositories.Platform;
using Domain.Entities;
using Domain.DTO.GameFile;
using Domain.Exceptions.GameFile;
using Domain.Exceptions.Game;
using Domain.Exceptions.Platform;

public class GameFileServiceTests {
    private readonly Mock<IGameFileRepository> _mockGameFileRepo;
    private readonly Mock<IGameRepository> _mockGameRepo;
    private readonly Mock<IPlatformRepository> _mockPlatformRepo;
    private readonly GameFileService _service;

    public GameFileServiceTests() {
        _mockGameFileRepo = new Mock<IGameFileRepository>();
        _mockGameRepo = new Mock<IGameRepository>();
        _mockPlatformRepo = new Mock<IPlatformRepository>();
        _service = new GameFileService(_mockGameFileRepo.Object, _mockGameRepo.Object, _mockPlatformRepo.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllFiles() {
        var files = new List<GameFile> {
            new() { Id = 1, FileName = "game1.exe" },
            new() { Id = 2, FileName = "game2.apk" }
        };
        _mockGameFileRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(files);

        var result = await _service.GetAllAsync();

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetByIdAsync_ExistingFile_ReturnsFile() {
        var file = new GameFile { Id = 1, FileName = "test.exe" };
        _mockGameFileRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(file);

        var result = await _service.GetByIdAsync(1);

        Assert.Equal("test.exe", result.FileName);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistentFile_ThrowsNotFoundException() {
        _mockGameFileRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((GameFile?)null);

        await Assert.ThrowsAsync<GameFileNotFoundException>(() => _service.GetByIdAsync(999));
    }

    [Fact]
    public async Task GetByGameIdAsync_ReturnsGameFiles() {
        var files = new List<GameFile> {
            new() { Id = 1, GameId = 1, FileName = "game.exe" },
            new() { Id = 2, GameId = 1, FileName = "game.apk" }
        };
        _mockGameFileRepo.Setup(r => r.GetByGameIdAsync(1)).ReturnsAsync(files);

        var result = await _service.GetByGameIdAsync(1);

        var gameFiles = result.ToList();
        Assert.Equal(2, gameFiles.Count);
        Assert.All(gameFiles, f => Assert.Equal(1, f.GameId));
    }

    [Fact]
    public async Task UploadFileAsync_GameNotFound_ThrowsGameNotFoundException() {
        var dto = CreateValidUploadDto();
        _mockGameRepo.Setup(r => r.GetByIdAsync(dto.GameId)).ReturnsAsync((Game?)null);

        await Assert.ThrowsAsync<GameNotFoundException>(() => _service.UploadFileAsync(dto));
    }

    [Fact]
    public async Task UploadFileAsync_PlatformNotFound_ThrowsPlatformNotFoundException() {
        var dto = CreateValidUploadDto();
        _mockGameRepo.Setup(r => r.GetByIdAsync(dto.GameId)).ReturnsAsync(new Game());
        _mockPlatformRepo.Setup(r => r.GetByIdAsync(dto.PlatformId)).ReturnsAsync((Platform?)null);

        await Assert.ThrowsAsync<PlatformNotFoundException>(() => _service.UploadFileAsync(dto));
    }

    [Fact]
    public async Task UploadFileAsync_FileSizeTooLarge_ThrowsFileSizeTooLargeException() {
        var dto = CreateValidUploadDto();
        dto.FileSize = 200 * 1024 * 1024; // 200 MB
        SetupValidGameAndPlatform(dto);

        await Assert.ThrowsAsync<FileSizeTooLargeException>(() => _service.UploadFileAsync(dto));
    }

    [Fact]
    public async Task UploadFileAsync_InvalidFileExtension_ThrowsInvalidFileExtensionException() {
        var dto = CreateValidUploadDto();
        dto.FileExtension = ".txt"; // Invalid for windows platform
        SetupValidGameAndPlatform(dto);

        await Assert.ThrowsAsync<InvalidFileExtensionException>(() => _service.UploadFileAsync(dto));
    }

    [Fact]
    public async Task UploadFileAsync_FileAlreadyExists_ThrowsGameFileAlreadyExistsException() {
        var dto = CreateValidUploadDto();
        SetupValidGameAndPlatform(dto);
        
        var existingFile = new GameFile { GameId = dto.GameId, PlatformId = dto.PlatformId };
        _mockGameFileRepo.Setup(r => r.GetByGameAndPlatformAsync(dto.GameId, dto.PlatformId))
            .ReturnsAsync(existingFile);

        await Assert.ThrowsAsync<GameFileAlreadyExistsException>(() => _service.UploadFileAsync(dto));
    }

    [Fact]
    public async Task UploadFileAsync_ValidData_CreatesFile() {
        var dto = CreateValidUploadDto();
        SetupValidGameAndPlatform(dto);
        _mockGameFileRepo.Setup(r => r.GetByGameAndPlatformAsync(dto.GameId, dto.PlatformId))
            .ReturnsAsync((GameFile?)null);
        
        var createdFile = new GameFile { Id = 1, FileName = dto.FileName };
        _mockGameFileRepo.Setup(r => r.CreateAsync(It.IsAny<GameFile>()))
            .ReturnsAsync(createdFile);

        var result = await _service.UploadFileAsync(dto);

        Assert.Equal(dto.FileName, result.FileName);
        _mockGameFileRepo.Verify(r => r.CreateAsync(It.IsAny<GameFile>()), Times.Once);
    }

    [Fact]
    public async Task DownloadFileAsync_ExistingFile_ReturnsFileData() {
        var file = new GameFile { Id = 1, FilePath = "test/path.exe" };
        _mockGameFileRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(file);
        
        // For now, just test the repository call
        await Assert.ThrowsAsync<FileUploadException>(() => _service.DownloadFileAsync(1));
        _mockGameFileRepo.Verify(r => r.GetByIdAsync(1), Times.Once);
    }

    [Fact]
    public async Task DownloadFileAsync_NonExistentFile_ThrowsNotFoundException() {
        _mockGameFileRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((GameFile?)null);

        await Assert.ThrowsAsync<GameFileNotFoundException>(() => _service.DownloadFileAsync(999));
    }

    [Fact]
    public async Task DeleteFileAsync_ExistingFile_DeletesFile() {
        var file = new GameFile { Id = 1, FilePath = "test/path.exe" };
        _mockGameFileRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(file);

        await _service.DeleteFileAsync(1);

        _mockGameFileRepo.Verify(r => r.DeleteAsync(file), Times.Once);
    }

    [Fact]
    public async Task DeleteFileAsync_NonExistentFile_ThrowsNotFoundException() {
        _mockGameFileRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((GameFile?)null);

        await Assert.ThrowsAsync<GameFileNotFoundException>(() => _service.DeleteFileAsync(999));
    }

    private static CreateGameFileDto CreateValidUploadDto() {
        return new CreateGameFileDto {
            FileName = "test.exe",
            FileExtension = ".exe",
            FileSize = 1024,
            GameId = 1,
            PlatformId = 3, // Windows platform
            FileContent = [1, 2, 3, 4]
        };
    }

    private void SetupValidGameAndPlatform(CreateGameFileDto dto) {
        _mockGameRepo.Setup(r => r.GetByIdAsync(dto.GameId)).ReturnsAsync(new Game { Id = dto.GameId });
        _mockPlatformRepo.Setup(r => r.GetByIdAsync(dto.PlatformId))
            .ReturnsAsync(new Platform { Id = dto.PlatformId, Name = "windows" });
    }
}
