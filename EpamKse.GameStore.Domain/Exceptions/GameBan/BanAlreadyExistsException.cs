namespace EpamKse.GameStore.Domain.Exceptions.GameBan;

public class BanAlreadyExistsException(int gameId, string country)
    : ConflictException($"Game {gameId} is already banned in {country}");
