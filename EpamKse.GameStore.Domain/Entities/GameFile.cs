namespace EpamKse.GameStore.Domain.Entities;

public class GameFile {
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string FileExtension { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public DateTime UploadedAt { get; set; }
    public int GameId { get; set; }
    public int PlatformId { get; set; }
}
