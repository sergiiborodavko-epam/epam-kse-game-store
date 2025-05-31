namespace EpamKse.GameStore.Domain.Exceptions.Platform;

public class PlatformNotFoundException(int id) : NotFoundException($"Platform with ID {id} not found.")
{
}