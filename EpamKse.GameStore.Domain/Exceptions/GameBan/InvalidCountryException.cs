namespace EpamKse.GameStore.Domain.Exceptions.GameBan;

public class InvalidCountryException(string country, string validCountries)
    : CustomHttpException(400, $"Invalid country: {country}. Valid countries are: {validCountries}");
