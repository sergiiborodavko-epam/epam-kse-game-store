namespace EpamKse.GameStore.Domain.Exceptions.GameFile;

public class GameFileAlreadyExistsException(int gameId, string platformName) : ConflictException($"File for game {gameId} on platform '{platformName}' already exists");
