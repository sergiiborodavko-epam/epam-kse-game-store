namespace EpamKse.GameStore.DataAccess.Repositories.Genre;

using Domain.Entities;

public interface IGenreRepository {
    Task<IEnumerable<Genre>> GetAllAsync();
    Task<Genre?> GetByIdAsync(int id);
    Task<Genre?> GetByNameAsync(string name);
    Task<List<Genre>> GetByNamesAsync(List<string> names);
    Task<List<Genre>> GetMainGenresAsync();
    Task<List<Genre>> GetSubGenresByParentNameAsync(string parentName);
    Task<Genre> CreateAsync(Genre genre);
    Task<Genre> UpdateAsync(Genre genre);
    Task DeleteAsync(Genre genre);
}
