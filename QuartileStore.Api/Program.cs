using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuartileStore.Api;
using QuartileStore.Api.Middleware;
using QuartileStore.Commons.Application.Dtos.Errors;
using QuartileStore.Commons.Domain.Constants;
using QuartileStore.Commons.Infrastructure.SqlServer.Contexts;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApi(builder.Configuration)
    .AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var validationErrors = context.ModelState
                .Where(e => e.Value!.Errors.Count > 0)
                .ToDictionary(
                    x => x.Key,
                    x => x.Value!.Errors.Select(e => e.ErrorMessage).ToArray());

            var errorResponse = new ApiErrorResponse
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Title = ApiErrorTitle.ValidationError,
                Detail = "The request failed due to one or more validation errors",
                Errors = validationErrors
            };

            return new BadRequestObjectResult(errorResponse);
        };
    });

var app = builder.Build();

app.UseMiddleware<ExceptionHandlerMiddleware>();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<QuartileDatabaseContext>();
    dbContext.Database.Migrate();
}

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();