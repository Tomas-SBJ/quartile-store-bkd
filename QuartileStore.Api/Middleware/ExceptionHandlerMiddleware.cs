using QuartileStore.Commons.Dtos.Errors;
using QuartileStore.Commons.Exceptions;

namespace QuartileStore.Api.Middleware;

public class ExceptionHandlerMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        ApiErrorResponse response;

        switch (exception)
        {
            case EntityNotFoundException entityNotFoundException:
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                response = new ApiErrorResponse
                {
                    StatusCode = context.Response.StatusCode,
                    Title = entityNotFoundException.Title,
                    Detail = entityNotFoundException.Message
                };
                break;
            
            case EntityAlreadyExistsException entityAlreadyExistsException:
                context.Response.StatusCode = StatusCodes.Status409Conflict;
                response = new ApiErrorResponse
                {
                    StatusCode = context.Response.StatusCode,
                    Title = entityAlreadyExistsException.Title,
                    Detail = entityAlreadyExistsException.Message
                };
                break;
            
            case DeleteConflictException deleteConflictException:
                context.Response.StatusCode = StatusCodes.Status409Conflict;
                response = new ApiErrorResponse
                {
                    StatusCode = context.Response.StatusCode,
                    Title = deleteConflictException.Title,
                    Detail = deleteConflictException.Message
                };
                break;
            
            default:
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                response = new ApiErrorResponse
                {
                    StatusCode = context.Response.StatusCode,
                    Title = "Internal Server Error",
                    Detail = "An internal server error has occurred"
                };
                break;
        }

        await context.Response.WriteAsJsonAsync(response);
    }
}