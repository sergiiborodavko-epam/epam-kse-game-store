namespace EpamKse.GameStore.Domain.Exceptions.Publisher;

public class PublisherDuplicationException(string name)
    : ConflictException($"Publisher with name {name} already exists.")
{
}