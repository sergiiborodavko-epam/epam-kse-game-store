namespace EpamKse.GameStore.Api.Tests.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Xunit;
using Moq;

using Api.Controllers;
using Dto;
using Domain.Entities;
using Domain.DTO.GameFile;
using Services.Services.GameFile;

public class GameFileControllerTests {
    private readonly Mock<IGameFileService> _mockService;
    private readonly GameFilesController _controller;

    public GameFileControllerTests() {
        _mockService = new Mock<IGameFileService>();
        _controller = new GameFilesController(_mockService.Object);
    }

    [Fact]
    public async Task GetAllFiles_ReturnsOkWithFilesList() {
        var files = new List<GameFile> {
            new() { Id = 1, FileName = "game1.exe", GameId = 1, PlatformId = 3 },
            new() { Id = 2, FileName = "game2.apk", GameId = 2, PlatformId = 1 }
        };
        _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(files);

        var result = await _controller.GetAllFiles();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedFiles = Assert.IsType<IEnumerable<GameFile>>(okResult.Value, exactMatch: false);
        Assert.Equal(2, returnedFiles.Count());
    }

    [Fact]
    public async Task GetFileById_ReturnsFileDetails() {
        var file = new GameFile { 
            Id = 1, FileName = "test.exe", FileExtension = ".exe", 
            FileSize = 1024, GameId = 1, PlatformId = 3 
        };
        _mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(file);

        var result = await _controller.GetFileById(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedFile = Assert.IsType<GameFile>(okResult.Value);
        Assert.Equal("test.exe", returnedFile.FileName);
    }

    [Fact]
    public async Task GetFilesByGameId_ReturnsGameFiles() {
        var files = new List<GameFile> {
            new() { Id = 1, FileName = "game.exe", GameId = 1, PlatformId = 3 },
            new() { Id = 2, FileName = "game.apk", GameId = 1, PlatformId = 1 }
        };
        _mockService.Setup(s => s.GetByGameIdAsync(1)).ReturnsAsync(files);

        var result = await _controller.GetFilesByGameId(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedFiles = Assert.IsType<IEnumerable<GameFile>>(okResult.Value, exactMatch: false);
        var gameFiles = returnedFiles.ToList();
        Assert.Equal(2, gameFiles.Count);
        Assert.All(gameFiles, f => Assert.Equal(1, f.GameId));
    }

    [Fact]
    public async Task UploadFile_ValidData_ReturnsCreated() {
        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.FileName).Returns("test.exe");
        mockFile.Setup(f => f.Length).Returns(1024);
        mockFile.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), CancellationToken.None))
            .Returns(Task.CompletedTask);

        var dto = new UploadGameFileDto {
            GameId = 1,
            PlatformId = 3,
            File = mockFile.Object
        };

        var createdFile = new GameFile { 
            Id = 1, FileName = "test.exe", GameId = 1, PlatformId = 3 
        };
        _mockService.Setup(s => s.UploadFileAsync(It.IsAny<CreateGameFileDto>()))
            .ReturnsAsync(createdFile);

        var result = await _controller.UploadFile(dto);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        var returnedFile = Assert.IsType<GameFile>(createdResult.Value);
        Assert.Equal("test.exe", returnedFile.FileName);
    }

    [Fact]
    public async Task DownloadFile_ValidId_ReturnsFile() {
        var fileData = new byte[] { 1, 2, 3, 4 };
        var fileInfo = new GameFile { Id = 1, FileName = "test.exe" };
        
        _mockService.Setup(s => s.DownloadFileAsync(1)).ReturnsAsync(fileData);
        _mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(fileInfo);

        var result = await _controller.DownloadFile(1);

        var fileResult = Assert.IsType<FileContentResult>(result);
        Assert.Equal("test.exe", fileResult.FileDownloadName);
        Assert.Equal("application/octet-stream", fileResult.ContentType);
        Assert.Equal(fileData, fileResult.FileContents);
    }

    [Fact]
    public async Task DeleteFile_ValidId_ReturnsNoContent() {
        _mockService.Setup(s => s.DeleteFileAsync(1)).Returns(Task.CompletedTask);

        var result = await _controller.DeleteFile(1);

        Assert.IsType<NoContentResult>(result);
        _mockService.Verify(s => s.DeleteFileAsync(1), Times.Once);
    }
}
