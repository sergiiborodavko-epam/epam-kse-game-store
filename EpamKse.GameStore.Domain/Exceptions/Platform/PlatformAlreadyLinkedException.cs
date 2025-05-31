namespace EpamKse.GameStore.Domain.Exceptions.Platform;

public class PlatformAlreadyLinkedException(int id)
    : ConflictException($"Platform with ID {id} is already linked to this publisher.")
{
}