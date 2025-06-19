namespace EpamKse.GameStore.Domain.Exceptions.License;

public class InvalidLicenseKeyException(string key) : BadRequestException($"Invalid license key: {key}");