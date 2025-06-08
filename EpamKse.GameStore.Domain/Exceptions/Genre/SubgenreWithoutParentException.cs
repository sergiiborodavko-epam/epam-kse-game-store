namespace EpamKse.GameStore.Domain.Exceptions.Genre;

public class SubgenreWithoutParentException(string subgenre) : CustomHttpException(400, $"Subgenre '{subgenre}' cannot be specified without its parent genre");
