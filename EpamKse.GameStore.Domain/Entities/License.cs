namespace EpamKse.GameStore.Domain.Entities;

public class License
{
    public int Id { get; set; }
    public string Key { get; set; }
    public int OrderId { get; set; }
    
    public Order Order { get; set; }
}