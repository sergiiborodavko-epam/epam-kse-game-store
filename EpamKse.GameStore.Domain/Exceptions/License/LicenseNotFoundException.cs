namespace EpamKse.GameStore.Domain.Exceptions.License;

public class LicenseNotFoundException(string message) : NotFoundException(message) {}