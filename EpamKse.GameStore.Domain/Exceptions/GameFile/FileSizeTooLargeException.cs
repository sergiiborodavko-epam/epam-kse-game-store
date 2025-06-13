namespace EpamKse.GameStore.Domain.Exceptions.GameFile;

public class FileSizeTooLargeException(long maxSize) : CustomHttpException(400, $"File size exceeds maximum allowed size of {maxSize} bytes");
