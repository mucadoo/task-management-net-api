using System.Net;
using System.Text.Json;
using TaskManagement.Domain.Exceptions;

namespace TaskManagement.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (TaskValidationException ex)
        {
            _logger.LogWarning(ex.Message);
            await WriteProblemDetails(context, (int)HttpStatusCode.BadRequest, "Validation Error", ex.Message);
        }
        catch (TaskNotFoundException ex)
        {
            _logger.LogWarning(ex.Message);
            await WriteProblemDetails(context, (int)HttpStatusCode.NotFound, "Not Found", ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred.");
            await WriteProblemDetails(context, (int)HttpStatusCode.InternalServerError, "Internal Server Error", "An unexpected error occurred.");
        }
    }

    private static async Task WriteProblemDetails(HttpContext context, int status, string title, string detail)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = status;

        var problemDetails = new
        {
            type = "",
            title = title,
            status = status,
            detail = detail,
            traceId = context.TraceIdentifier
        };

        var result = JsonSerializer.Serialize(problemDetails);
        await context.Response.WriteAsync(result);
    }
}
