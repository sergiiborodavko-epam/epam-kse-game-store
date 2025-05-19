using EpamKse.GameStore.Api.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EpamKse.GameStore.Api.Filters;

public class CustomHttpExceptionFilter : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        if (context.Exception is CustomHttpException ex)
        {
            context.Result = new ObjectResult(new { message = ex.Message }) { StatusCode = ex.StatusCode };
        }
    }
}