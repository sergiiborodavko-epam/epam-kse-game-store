namespace EpamKse.GameStore.DataAccess.Repositories.Publisher;

using Domain.Entities;

public interface IPublisherRepository
{
    Task<Publisher?> GetByIdAsync(int id);
    Task<Publisher?> GetByNameAsync(string name);
    Task<List<Publisher>> GetPaginatedFullAsync(int skip, int take);
    Task<Publisher> AddAsync(Publisher publisher);
    Task RemoveAsync(Publisher publisher);

    Task<Publisher> UpdateAsync(Publisher publisher);
}