namespace EpamKse.GameStore.Domain.Entities;

public class LicenseKey
{
    public int OrderId { get; set; }
    public IEnumerable<int> GameIds = [];
    public int UserId { get; set; }
}