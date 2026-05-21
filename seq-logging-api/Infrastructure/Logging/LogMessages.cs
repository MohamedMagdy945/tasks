// ============================================================
//  Infrastructure/Logging/LogMessages.cs
//
//  Compile-time log message definitions using the
//  [LoggerMessage] source generator (available since .NET 6).
//
//  WHY bother?
//  ───────────
//  Calling _logger.LogInformation("msg {Arg}", arg) allocates
//  memory on every call — even when the log level is disabled.
//  The source generator produces a cached, allocation-free
//  delegate instead, which is measurably faster in hot paths.
//
//  RULES
//  ─────
//  • The class must be partial.
//  • The method must be partial, static, and return void.
//  • The first parameter must be ILogger.
//  • EventId should be unique per class; use a range per layer
//    (1000s for orders, 2000s for payments, etc.).
// ============================================================

using Microsoft.Extensions.Logging;

namespace SeqLoggingApi.Infrastructure.Logging;

// ── Order domain messages ─────────────────────────────────
public static partial class OrderLogMessages
{
    [LoggerMessage(EventId = 1001, Level = LogLevel.Information,
        Message = "Creating order for customer {CustomerId} with {ItemCount} item(s)")]
    public static partial void CreatingOrder(
        ILogger logger, string customerId, int itemCount);

    [LoggerMessage(EventId = 1002, Level = LogLevel.Information,
        Message = "Order {OrderId} created successfully for customer {CustomerId}")]
    public static partial void OrderCreated(
        ILogger logger, int orderId, string customerId);

    [LoggerMessage(EventId = 1003, Level = LogLevel.Information,
        Message = "Retrieving order {OrderId}")]
    public static partial void RetrievingOrder(ILogger logger, int orderId);

    [LoggerMessage(EventId = 1004, Level = LogLevel.Warning,
        Message = "Order {OrderId} not found")]
    public static partial void OrderNotFound(ILogger logger, int orderId);

    [LoggerMessage(EventId = 1005, Level = LogLevel.Error,
        Message = "Failed to process order {OrderId}")]
    public static partial void OrderProcessingFailed(
        ILogger logger, Exception ex, int orderId);

    [LoggerMessage(EventId = 1006, Level = LogLevel.Warning,
        Message = "Order {OrderId} validation failed: {Reason}")]
    public static partial void OrderValidationFailed(
        ILogger logger, int orderId, string reason);

    [LoggerMessage(EventId = 1007, Level = LogLevel.Information,
        Message = "Cancelling order {OrderId} — reason: {Reason}")]
    public static partial void CancellingOrder(
        ILogger logger, int orderId, string reason);
}

// ── General API messages ──────────────────────────────────
public static partial class ApiLogMessages
{
    [LoggerMessage(EventId = 9001, Level = LogLevel.Warning,
        Message = "Request validation failed for {Endpoint}: {Errors}")]
    public static partial void ValidationFailed(
        ILogger logger, string endpoint, string errors);

    [LoggerMessage(EventId = 9002, Level = LogLevel.Error,
        Message = "Unhandled exception on {Method} {Path}")]
    public static partial void UnhandledException(
        ILogger logger, Exception ex, string method, string path);

    [LoggerMessage(EventId = 9003, Level = LogLevel.Information,
        Message = "Correlation ID assigned: {CorrelationId}")]
    public static partial void CorrelationIdAssigned(
        ILogger logger, string correlationId);
}
