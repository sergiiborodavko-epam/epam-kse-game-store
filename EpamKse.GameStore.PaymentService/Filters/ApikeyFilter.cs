using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EpamKse.GameStore.PaymentService.Filters;

public class ApikeyFilter : IAuthorizationFilter
{
    private readonly string _apiKey;

    public ApikeyFilter(string apiKey)
    {
        _apiKey = apiKey;
    }
    
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var apiKey = context.HttpContext.Request.Headers["X-API-KEY"].ToString();
        if (apiKey != _apiKey)
        {
            context.Result = new UnauthorizedResult();
        }
    }
}