using Microsoft.AspNetCore.Http;

namespace EpamKse.GameStore.Services.Helpers.Auth;

public static class AddRefreshTokenHelper
{
    private static readonly Int32 COOKIE_LIFETIME = 7;

    public static void AddRefreshTokenToCookies(this IResponseCookies cookies, string refreshToken)
    {
        cookies.Append("refreshToken", refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddDays(COOKIE_LIFETIME)
        });
    }
}