namespace EpamKse.GameStore.Domain.Entities;

public class LicenseKey
{
    public int OrderId { get; set; }
    public IEnumerable<int> GameIds { get; set; } = [];
    public int UserId { get; set; }
}