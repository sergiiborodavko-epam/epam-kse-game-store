using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace EpamKse.GameStore.Api.Infrastructure;

public class RefreshTokenValidator
{
    private readonly RequestDelegate _next;
    private readonly string _refreshTokenSecret;

    public RefreshTokenValidator(RequestDelegate next)
    {
        _next = next;
        _refreshTokenSecret = Environment.GetEnvironmentVariable("REFRESH_TOKEN_SECRET");
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var refreshToken = context.Request.Cookies["refreshToken"];

        if (!string.IsNullOrEmpty(refreshToken))
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_refreshTokenSecret);

                var principal = tokenHandler.ValidateToken(refreshToken, new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                context.Items["RefreshTokenClaims"] = principal;
            }
            catch (Exception)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Invalid refresh token.");
                return;
            }
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Missing refresh token.");
            return;
        }

        await _next(context);
    }
}