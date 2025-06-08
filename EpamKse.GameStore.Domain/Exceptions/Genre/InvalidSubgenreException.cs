namespace EpamKse.GameStore.Domain.Exceptions.Genre;

public class InvalidSubgenreException(string subgenre, string parentGenre) : CustomHttpException(400, $"Subgenre '{subgenre}' does not belong to genre '{parentGenre}'");
