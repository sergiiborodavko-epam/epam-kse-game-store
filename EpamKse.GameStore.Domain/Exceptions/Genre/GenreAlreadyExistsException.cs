namespace EpamKse.GameStore.Domain.Exceptions.Genre;

public class GenreAlreadyExistsException(string genreName) : ConflictException($"Genre '{genreName}' already exists.");
