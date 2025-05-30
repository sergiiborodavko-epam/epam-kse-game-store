namespace EpamKse.GameStore.DataAccess.Repositories.User;

using Domain.Entities;

public interface IUserRepository {
    Task<IEnumerable<User>> GetAllAsync();
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByEmail(string email);
    Task<User> CreateAsync(User user);
    Task<User> UpdateAsync(User user);
    Task DeleteAsync(User user);
}
