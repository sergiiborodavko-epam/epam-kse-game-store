namespace EpamKse.GameStore.Domain.Exceptions.Platform;

public class PlatformIsNotLinkedException(int id)
    : ConflictException($"Platform with ID {id} is not linked to this publisher.")
{
}