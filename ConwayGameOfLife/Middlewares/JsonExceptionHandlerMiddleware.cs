using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace ConwayGameOfLife.Middlewares;

public class JsonExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public JsonExceptionHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (BadHttpRequestException ex) when (ex.InnerException is JsonException)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/problem+json";

            var problem = new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Invalid JSON format",
                Detail = "The request body contains invalid JSON. Please check for syntax errors.",
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
            };

            await JsonSerializer.SerializeAsync(
                context.Response.Body,
                problem,
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                }
            );
        }
    } 
}