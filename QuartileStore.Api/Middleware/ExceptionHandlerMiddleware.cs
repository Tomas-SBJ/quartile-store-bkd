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

        object response;

        switch (exception)
        {
            case EntityNotFoundException entityNotFoundException:
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                response = new { error = entityNotFoundException.Message };
                break;
            
            case EntityAlreadyExistsException entityAlreadyExistsException:
                context.Response.StatusCode = StatusCodes.Status409Conflict;
                response = new { error = entityAlreadyExistsException.Message };
                break;
            
            default:
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                response = new { error = "An internal server error has occurred" };
                break;
        }

        await context.Response.WriteAsJsonAsync(response);
    }
}