namespace EpamKse.GameStore.Domain.Exceptions.Genre;

public class GenresNotFoundException(IEnumerable<string> names) : NotFoundException($"Genres with names '{string.Join(", ", names)}' not found.");
