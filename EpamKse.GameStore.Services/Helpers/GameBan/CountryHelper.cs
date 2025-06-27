namespace EpamKse.GameStore.Services.Helpers.GameBan;

using Domain.Enums;

public static class CountryHelper {
    public static string GetValidCountries() {
        return string.Join(", ", Enum.GetNames<Countries>());
    }

    public static bool IsValidCountry(string country) {
        return Enum.TryParse<Countries>(country, true, out _);
    }

    public static Countries ParseCountry(string country) {
        return Enum.Parse<Countries>(country, true);
    }
}
