using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Logging;
using QuartileStore.Commons.Application.Dtos.Errors;
using QuartileStore.Commons.Application.Exceptions;

namespace QuartileStore.Products.Functions.Middleware;

public class FunctionsExceptionHandlerMiddleware(
    ILogger<FunctionsExceptionHandlerMiddleware> logger
) : IFunctionsWorkerMiddleware
{
    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "An unhandled exception occurred");

            var httpRequest = await context.GetHttpRequestDataAsync();

            if (httpRequest is null)
                return;

            HttpResponseData response;
            ApiErrorResponse apiErrorResponse;

            switch (exception)
            {
                case EntityNotFoundException entityNotFoundException:
                    response = httpRequest.CreateResponse(HttpStatusCode.NotFound);
                    apiErrorResponse = new ApiErrorResponse
                    {
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Title = entityNotFoundException.Title,
                        Detail = entityNotFoundException.Message
                    };
                    break;

                case EntityAlreadyExistsException entityAlreadyExistsException:
                    response = httpRequest.CreateResponse(HttpStatusCode.Conflict);
                    apiErrorResponse = new ApiErrorResponse
                    {
                        StatusCode = (int)HttpStatusCode.Conflict,
                        Title = entityAlreadyExistsException.Title,
                        Detail = entityAlreadyExistsException.Message
                    };
                    break;

                case DeleteConflictException deleteConflictException:
                    response = httpRequest.CreateResponse(HttpStatusCode.Conflict);
                    apiErrorResponse = new ApiErrorResponse
                    {
                        StatusCode = (int)HttpStatusCode.Conflict,
                        Title = deleteConflictException.Title,
                        Detail = deleteConflictException.Message
                    };
                    break;

                default:
                    response = httpRequest.CreateResponse(HttpStatusCode.InternalServerError);
                    apiErrorResponse = new ApiErrorResponse
                    {
                        StatusCode = (int)HttpStatusCode.InternalServerError,
                        Title = "Internal Server Error",
                        Detail = "An internal server error has occurred"
                    };
                    break;
            }

            await response.WriteAsJsonAsync(apiErrorResponse);

            context.GetInvocationResult().Value = response;
        }
    }
}