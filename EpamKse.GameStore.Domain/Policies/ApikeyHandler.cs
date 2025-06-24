using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace EpamKse.GameStore.Domain.Policies;

public class ApikeyRequirement(string apiKey) : IAuthorizationRequirement
{
    public string Apikey { get; } = apiKey;
}

public class ApikeyHandler : AuthorizationHandler<ApikeyRequirement>
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ApikeyHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ApikeyRequirement requirement)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var apiKey = httpContext.Request.Headers["X-API-KEY"].ToString();
        
        if (apiKey == requirement.Apikey)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}