namespace EpamKse.GameStore.Domain.Exceptions.GameFile;

public class InvalidFileExtensionException(string extension, string platform) : CustomHttpException(400, $"File extension '{extension}' is not valid for platform '{platform}'");
