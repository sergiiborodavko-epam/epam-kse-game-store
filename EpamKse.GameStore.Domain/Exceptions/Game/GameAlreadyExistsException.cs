namespace EpamKse.GameStore.Domain.Exceptions.Game;

public class GameAlreadyExistsException(string title) : ConflictException($"Game with title '{title}' already exists");
