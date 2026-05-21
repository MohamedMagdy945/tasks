// ============================================================
//  Infrastructure/Logging/LogContextExtensions.cs
//
//  Extension methods that make it easy to push structured
//  properties into Serilog's ambient LogContext from anywhere
//  in the code (controllers, services, middleware …).
//
//  Properties pushed here appear on EVERY log event emitted
//  within the same async flow until the returned IDisposable
//  is disposed.
// ============================================================

using Serilog.Context;

namespace SeqLoggingApi.Infrastructure.Logging;

public static class LogContextExtensions
{
    /// <summary>
    /// Pushes a CorrelationId into the log context.
    /// Use this at the start of a request or background job so
    /// that every log line for that unit of work is linked.
    /// </summary>
    public static IDisposable PushCorrelationId(string correlationId) =>
        LogContext.PushProperty("CorrelationId", correlationId);

    /// <summary>
    /// Pushes the authenticated user's ID into the log context.
    /// </summary>
    public static IDisposable PushUserId(string userId) =>
        LogContext.PushProperty("UserId", userId);

    /// <summary>
    /// Pushes an arbitrary key/value pair.
    /// </summary>
    public static IDisposable Push(string key, object value) =>
        LogContext.PushProperty(key, value);

    /// <summary>
    /// Pushes multiple properties at once and returns a combined
    /// disposable that removes all of them when disposed.
    /// </summary>
    public static IDisposable PushMany(params (string Key, object Value)[] props)
    {
        // Stack the disposables so disposing the outer one
        // walks back through all pushed properties in order.
        IDisposable? combined = null;
        foreach (var (key, value) in props)
        {
            var pushed = LogContext.PushProperty(key, value);
            combined = combined is null
                ? pushed
                : new CombinedDisposable(combined, pushed);
        }
        return combined ?? new CombinedDisposable();
    }

    // ── Internal helper ──────────────────────────────────────
    private sealed class CombinedDisposable(params IDisposable[] items) : IDisposable
    {
        public void Dispose()
        {
            foreach (var item in items)
                item.Dispose();
        }
    }
}
