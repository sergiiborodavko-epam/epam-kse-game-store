namespace EpamKse.GameStore.Services.Helpers.GameBan;

using Domain.Enums;

public static class CountryHelper {
    public static string GetValidCountries() {
        return string.Join(", ", Enum.GetNames<Countries>());
    }

    public static bool IsValidCountry(Countries country) {
        return Enum.IsDefined(typeof(Countries), country);
    }
}
