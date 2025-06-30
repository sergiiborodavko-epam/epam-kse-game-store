using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EpamKse.GameStore.PaymentService.Filters;

public class ApikeyFilter(string key) : IAuthorizationFilter {
    public void OnAuthorization(AuthorizationFilterContext context) {
        if (context.HttpContext.Request.Headers.TryGetValue("x-api-key", out var apiKeyValues)) {
            var apiKey = apiKeyValues.FirstOrDefault();
            if (!string.IsNullOrEmpty(apiKey) && apiKey == key) {
                return;
            }
        }
    
        context.Result = new UnauthorizedResult();
    }
}
