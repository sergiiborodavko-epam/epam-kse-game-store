namespace EpamKse.GameStore.Domain.Exceptions.Genre;

public class GenreAlreadyExistsException(string genreName) : ConflictException($"Genre '{genreName}' already exists.");
public class GenreIdNotFoundException(int id) : NotFoundException($"Genre with ID '{id}' not found.");
public class GenreNamesNotFoundException(IEnumerable<string> names) : NotFoundException($"Genres with names '{string.Join(", ", names)}' not found.");
