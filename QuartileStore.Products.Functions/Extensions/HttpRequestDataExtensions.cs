using System.ComponentModel.DataAnnotations;
using System.Net;
using Microsoft.Azure.Functions.Worker.Http;
using QuartileStore.Commons.Domain.Constants;
using QuartileStore.Commons.Dtos.Errors;

namespace QuartileStore.Products.Functions.Extensions;

public static class HttpRequestDataExtensions
{
    public static async Task<(T? Dto, HttpResponseData? ErrorResponse)> ReadAndValidateJsonAsync<T>(
        this HttpRequestData req)
    {
        var dto = await req.ReadFromJsonAsync<T>();

        if (dto is null)
        {
            var badRequestResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badRequestResponse.WriteAsJsonAsync(new ApiErrorResponse
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Title = ApiErrorTitle.ValidationError,
                Detail = "The body of the request cannot be empty"
            });
            return (default, badRequestResponse);
        }

        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(dto, null, null);
        var isValid = Validator.TryValidateObject(dto, validationContext, validationResults, true);

        if (!isValid)
        {
            var errorDictionary = validationResults
                .SelectMany(vr => vr.MemberNames.DefaultIfEmpty(string.Empty), (vr, memberName) => new
                {
                    FieldName = ToCamelCase(memberName),
                    ErrorMessage = vr.ErrorMessage
                })
                .GroupBy(x => x.FieldName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.ErrorMessage!).ToArray()
                );

            var errorResponse = new ApiErrorResponse
            {
                StatusCode = 400,
                Title = ApiErrorTitle.ValidationError,
                Detail = "The request failed due to one or more validation errors",
                Errors = errorDictionary
            };

            var badRequestResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badRequestResponse.WriteAsJsonAsync(errorResponse);

            return (default, badRequestResponse);
        }

        return (dto, null);
    }

    private static string ToCamelCase(string str)
    {
        if (!string.IsNullOrEmpty(str) && char.IsUpper(str[0]))
        {
            return char.ToLowerInvariant(str[0]) + str.Substring(1);
        }

        return str;
    }
}