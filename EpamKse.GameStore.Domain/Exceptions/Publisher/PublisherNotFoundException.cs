namespace EpamKse.GameStore.Domain.Exceptions.Publisher;

public class PublisherNotFoundException(int id) : NotFoundException($"Publisher with ID {id} not found.")
{
}