namespace EpamKse.GameStore.Domain.Exceptions.GameFile;

public class FileUploadException(string message) : CustomHttpException(500, $"File upload failed: {message}");
