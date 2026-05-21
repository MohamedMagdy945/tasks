// ============================================================
//  Infrastructure/Middleware/GlobalExceptionMiddleware.cs
//
//  Catches any unhandled exception that escapes the controller
//  layer, logs it with full structured context, then returns
//  a clean RFC 7807 "Problem Details" JSON response instead of
//  leaking internal stack traces to API consumers.
// ============================================================

using System.Net;
using System.Text.Json;
using SeqLoggingApi.Infrastructure.Logging;

namespace SeqLoggingApi.Infrastructure.Middleware;

public sealed class GlobalExceptionMiddleware(
    RequestDelegate         next,
    ILogger<GlobalExceptionMiddleware> logger)
{
    // Shared serialiser options — reused across all error responses.
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

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

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        // Determine HTTP status and error code based on exception type.
        var (statusCode, errorCode) = ex switch
        {
            ArgumentException       or
            InvalidOperationException  => (HttpStatusCode.BadRequest,         "BAD_REQUEST"),
            KeyNotFoundException       => (HttpStatusCode.NotFound,            "NOT_FOUND"),
            UnauthorizedAccessException=> (HttpStatusCode.Unauthorized,        "UNAUTHORIZED"),
            TimeoutException           => (HttpStatusCode.GatewayTimeout,      "TIMEOUT"),
            _                          => (HttpStatusCode.InternalServerError,  "INTERNAL_ERROR")
        };

        // Log with full context.  The log entry in Seq will contain:
        //   - The exception object (message + stack trace)
        //   - HTTP method & path
        //   - Error code and status
        //   - Correlation ID (from LogContext, pushed by middleware earlier)
        ApiLogMessages.UnhandledException(
            logger, ex,
            context.Request.Method,
            context.Request.Path);

        // Build an RFC 7807 Problem Details response.
        var problem = new
        {
            type     = $"https://httpstatuses.com/{(int)statusCode}",
            title    = errorCode,
            status   = (int)statusCode,
            detail   = ex.Message,
            // Only expose the instance (path) so consumers can correlate
            // this with their own request logs.
            instance = context.Request.Path.Value
        };

        context.Response.StatusCode  = (int)statusCode;
        context.Response.ContentType = "application/problem+json";

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(problem, _jsonOptions));
    }
}
