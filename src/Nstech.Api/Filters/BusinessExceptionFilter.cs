using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Nstech.Domain.Common;

namespace Nstech.Api.Filters;

public sealed class BusinessExceptionFilter : IAsyncExceptionFilter
{
    public Task OnExceptionAsync(ExceptionContext context)
    {
        if (context.Exception is BusinessException businessException)
        {
            context.Result = new BadRequestObjectResult(new
            {
                error = new
                {
                    code = businessException.Code,
                    message = businessException.Message
                }
            });
            context.ExceptionHandled = true;
            return Task.CompletedTask;
        }

        context.Result = new ObjectResult(new
        {
            error = new
            {
                code = "UNHANDLED_ERROR",
                message = "An unexpected error occurred."
            }
        })
        {
            StatusCode = StatusCodes.Status500InternalServerError
        };
        context.ExceptionHandled = true;
        return Task.CompletedTask;
    }
}
