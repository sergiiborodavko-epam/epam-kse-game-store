using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EpamKse.GameStore.Api.Controllers;

using Dto;
using Domain.DTO.GameFile;
using Services.Services.GameFile;

[ApiController]
[Route("api/game-files")]
public class GameFilesController(IGameFileService gameFileService) : ControllerBase {

    [HttpGet]
    public async Task<IActionResult> GetAllFiles() {
        var files = await gameFileService.GetAllAsync();
        return Ok(files);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetFileById(int id) {
        var file = await gameFileService.GetByIdAsync(id);
        return Ok(file);
    }

    [HttpGet("game/{gameId:int}")]
    public async Task<IActionResult> GetFilesByGameId(int gameId) {
        var files = await gameFileService.GetByGameIdAsync(gameId);
        return Ok(files);
    }

    [HttpPost("upload")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UploadFile([FromForm] UploadGameFileDto dto) {
        if (!ModelState.IsValid) {
            return BadRequest(ModelState);
        }

        using var memoryStream = new MemoryStream();
        await dto.File.CopyToAsync(memoryStream);

        var createDto = new CreateGameFileDto {
            FileName = dto.File.FileName,
            FileExtension = Path.GetExtension(dto.File.FileName),
            FileSize = dto.File.Length,
            GameId = dto.GameId,
            PlatformId = dto.PlatformId,
            FileContent = memoryStream.ToArray()
        };

        var result = await gameFileService.UploadFileAsync(createDto);
        return CreatedAtAction(nameof(GetFileById), new { id = result.Id }, result);
    }

    [HttpGet("download/{id:int}")]
    public async Task<IActionResult> DownloadFile(int id) {
        var fileData = await gameFileService.DownloadFileAsync(id);
        var fileInfo = await gameFileService.GetByIdAsync(id);
        
        return File(fileData, "application/octet-stream", fileInfo.FileName);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteFile(int id) {
        await gameFileService.DeleteFileAsync(id);
        return NoContent();
    }
}
