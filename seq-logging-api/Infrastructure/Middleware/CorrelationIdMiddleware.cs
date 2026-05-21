// ============================================================
//  Infrastructure/Middleware/CorrelationIdMiddleware.cs
//
//  Ensures every inbound request has a Correlation ID:
//    • If the caller sends "X-Correlation-Id" header → use it.
//    • Otherwise → generate a new GUID.
//
//  The ID is then:
//    1. Pushed into Serilog's LogContext so it appears on every
//       log event emitted during this request.
//    2. Added to the response headers so the caller can match
//       their client-side logs to your server-side logs in Seq.
// ============================================================

using SeqLoggingApi.Infrastructure.Logging;

namespace SeqLoggingApi.Infrastructure.Middleware;

public sealed class CorrelationIdMiddleware(RequestDelegate next,
    ILogger<CorrelationIdMiddleware> logger)
{
    private const string HeaderName = "X-Correlation-Id";

    public async Task InvokeAsync(HttpContext context)
    {
        // 1. Resolve or create the correlation ID.
        var correlationId = context.Request.Headers[HeaderName].FirstOrDefault()
                         ?? Guid.NewGuid().ToString("N");

        // 2. Expose it on the response so consumers can read it.
        context.Response.Headers[HeaderName] = correlationId;

        // 3. Push into Serilog ambient context.
        //    Every log event inside the using block will carry CorrelationId.
        using (LogContextExtensions.PushCorrelationId(correlationId))
        {
            ApiLogMessages.CorrelationIdAssigned(logger, correlationId);
            await next(context);
        }
    }
}
