using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace EpamKse.GameStore.Domain.Policies;

public class ApikeyRequirement(string apiKey) : IAuthorizationRequirement {
    public string Apikey { get; } = apiKey;
}

public class ApikeyHandler(IHttpContextAccessor httpContextAccessor) : AuthorizationHandler<ApikeyRequirement> {
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ApikeyRequirement requirement) {
        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext == null) {
            return Task.CompletedTask;
        }

        if (httpContext.Request.Headers.TryGetValue("x-api-key", out var apiKeyValues)) {
            var apiKey = apiKeyValues.FirstOrDefault();
            if (!string.IsNullOrEmpty(apiKey) && apiKey == requirement.Apikey) {
                context.Succeed(requirement);
            }
        }

        return Task.CompletedTask;
    }
}
