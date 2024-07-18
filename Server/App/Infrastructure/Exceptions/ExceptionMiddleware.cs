using App.Infrastructure.Exceptions.Base;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace App.Infrastructure.Exceptions;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next; 
    private readonly Dictionary<Type, int> _exceptionStatusCodes = new()
    {
        { typeof(NotFoundException), StatusCodes.Status404NotFound },
        //{ typeof(FluentValidationException), StatusCodes.Status400BadRequest },
        { typeof(DbUpdateException), StatusCodes.Status409Conflict },
        { typeof(InvalidOperationException), StatusCodes.Status500InternalServerError },
        { typeof(ArgumentNullException), StatusCodes.Status400BadRequest },
        { typeof(NullReferenceException), StatusCodes.Status400BadRequest },
        { typeof(ArgumentOutOfRangeException), StatusCodes.Status400BadRequest },
        { typeof(ArgumentException), StatusCodes.Status400BadRequest },
        //{ typeof(ValidationAsyncException), StatusCodes.Status400BadRequest },
        { typeof(Exception), StatusCodes.Status500InternalServerError }
    };

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context); 
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex); 
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json"; 

        var exceptionType = ex.GetType(); 

        if (_exceptionStatusCodes.ContainsKey(exceptionType))
        {
            context.Response.StatusCode = _exceptionStatusCodes[exceptionType]; 
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        }

        return context.Response.WriteAsync(JsonConvert.SerializeObject(new ErrorDetails() 
        {
            StatusCode = context.Response.StatusCode,
            Message = ex.Message
        }));
    }
}