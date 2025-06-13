namespace EpamKse.GameStore.Domain.DTO.GameFile;

public class CreateGameFileDto {
    public string FileName { get; set; } = string.Empty;
    public string FileExtension { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public int GameId { get; set; }
    public int PlatformId { get; set; }
    public byte[] FileContent { get; set; } = [];
}
