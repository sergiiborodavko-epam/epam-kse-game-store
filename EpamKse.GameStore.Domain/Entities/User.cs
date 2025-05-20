namespace EpamKse.GameStore.Domain.Entities;

using Enums;

public class User
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public DateTime CreatedAt { get; set; }
    public Roles Role { get; set; }
    public string PasswordHash { get; set; }
    public string FullName { get; set; }
}