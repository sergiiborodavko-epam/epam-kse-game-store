namespace EpamKse.GameStore.DataAccess.Repositories.Genre;

using Microsoft.EntityFrameworkCore;

using Context;
using Domain.Entities;

public class GenreRepository(GameStoreDbContext context) : IGenreRepository {

    public async Task<IEnumerable<Genre>> GetAllAsync() {
        return await context.Genres.ToListAsync();
    }

    public async Task<Genre?> GetByIdAsync(int id) {
        return await context.Genres.FindAsync(id);
    }
    
    public async Task<Genre?> GetByNameAsync(string name) {
        return await context.Genres.FirstOrDefaultAsync(g => g.Name == name);
    }

    public async Task<List<Genre>> GetByNamesAsync(List<string> names) {
        return await context.Genres
            .Where(g => names.Contains(g.Name))
            .ToListAsync();
    }

    public async Task<Genre> CreateAsync(Genre genre) {
        context.Genres.Add(genre);
        await context.SaveChangesAsync();
        return genre;
    }

    public async Task<Genre> UpdateAsync(Genre genre) {
        context.Genres.Update(genre);
        await context.SaveChangesAsync();
        return genre;
    }

    public async Task DeleteAsync(Genre genre) {
        context.Genres.Remove(genre);
        await context.SaveChangesAsync();
    }
}
