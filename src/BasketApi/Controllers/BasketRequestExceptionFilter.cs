using BasketApi.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

public class RequestExceptionFilter : IActionFilter, IOrderedFilter
{
    public int Order { get; } = int.MaxValue - 10;

    public void OnActionExecuting(ActionExecutingContext context) { }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception is BasketRequestException exception)
        {
            context.Result = new ObjectResult(exception.Message)
            {
                StatusCode = 400,
            };
            context.ExceptionHandled = true;
        }
    }
}