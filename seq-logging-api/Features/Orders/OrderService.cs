// ============================================================
//  Features/Orders/OrderService.cs
//
//  Business logic layer — every important action is logged
//  using the [LoggerMessage] source-generated methods from
//  LogMessages.cs.  The service also demonstrates:
//
//   • Structured properties on every log event
//   • LogContext scoping (all logs inside a method share OrderId)
//   • Warning on validation failure
//   • Error (with exception) on unexpected failures
//   • Performance timing via Stopwatch
// ============================================================

using System.Diagnostics;
using SeqLoggingApi.Infrastructure.Logging;
using SeqLoggingApi.Models;

namespace SeqLoggingApi.Features.Orders;

// ── Interface ─────────────────────────────────────────────
public interface IOrderService
{
    Task<OrderResponse>        CreateAsync(CreateOrderRequest request);
    Task<OrderResponse>        GetByIdAsync(int id);
    Task<IEnumerable<OrderResponse>> GetAllAsync();
    Task<OrderResponse>        CancelAsync(int id, string reason);
}

// ── Implementation ────────────────────────────────────────
public sealed class OrderService(ILogger<OrderService> logger) : IOrderService
{
    // In-memory store — replace with your DbContext in a real app.
    private static readonly Dictionary<int, Order> _store = [];
    private static int _nextId = 1;

    // ── CreateAsync ──────────────────────────────────────
    public Task<OrderResponse> CreateAsync(CreateOrderRequest request)
    {
        // Push CustomerId into ambient context.
        // Every log event inside this using block will carry CustomerId.
        using (LogContextExtensions.Push("CustomerId", request.CustomerId))
        {
            OrderLogMessages.CreatingOrder(logger, request.CustomerId, request.Items.Count);

            // Validation
            if (string.IsNullOrWhiteSpace(request.CustomerId))
            {
                OrderLogMessages.OrderValidationFailed(logger, 0, "CustomerId is required");
                throw new ArgumentException("CustomerId is required.");
            }
            if (request.Items.Count == 0)
            {
                OrderLogMessages.OrderValidationFailed(logger, 0, "Order must have at least one item");
                throw new ArgumentException("Order must contain at least one item.");
            }

            var sw = Stopwatch.StartNew();

            var order = new Order
            {
                Id         = _nextId++,
                CustomerId = request.CustomerId,
                TotalAmount= request.Items.Sum(i => i.Quantity * i.UnitPrice),
                Items      = request.Items.Select(i => new OrderItem
                {
                    ProductId = i.ProductId,
                    Name      = i.Name,
                    Quantity  = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            };

            _store[order.Id] = order;
            sw.Stop();

            // Log completion — note the extra structured properties:
            // these become searchable columns in Seq.
            using (logger.BeginScope(new Dictionary<string, object>
            {
                ["OrderId"]        = order.Id,
                ["TotalAmount"]    = order.TotalAmount,
                ["DurationMs"]     = sw.ElapsedMilliseconds
            }))
            {
                OrderLogMessages.OrderCreated(logger, order.Id, order.CustomerId);
            }

            return Task.FromResult(MapToResponse(order));
        }
    }

    // ── GetByIdAsync ─────────────────────────────────────
    public Task<OrderResponse> GetByIdAsync(int id)
    {
        OrderLogMessages.RetrievingOrder(logger, id);

        if (!_store.TryGetValue(id, out var order))
        {
            OrderLogMessages.OrderNotFound(logger, id);
            throw new KeyNotFoundException($"Order {id} was not found.");
        }

        return Task.FromResult(MapToResponse(order));
    }

    // ── GetAllAsync ──────────────────────────────────────
    public Task<IEnumerable<OrderResponse>> GetAllAsync()
    {
        // Fine-grained debug log — won't appear in Production
        // because the minimum level there is Warning for this namespace.
        logger.LogDebug("Listing all orders. Current count: {Count}", _store.Count);

        return Task.FromResult(_store.Values.Select(MapToResponse));
    }

    // ── CancelAsync ──────────────────────────────────────
    public Task<OrderResponse> CancelAsync(int id, string reason)
    {
        if (!_store.TryGetValue(id, out var order))
        {
            OrderLogMessages.OrderNotFound(logger, id);
            throw new KeyNotFoundException($"Order {id} was not found.");
        }

        if (order.Status == "Cancelled")
        {
            // A warning: not a crash, but the caller should know.
            OrderLogMessages.OrderValidationFailed(logger, id, "Order is already cancelled");
            throw new InvalidOperationException($"Order {id} is already cancelled.");
        }

        OrderLogMessages.CancellingOrder(logger, id, reason);

        order.Status = "Cancelled";

        logger.LogInformation(
            "Order {OrderId} cancelled. Status changed to {NewStatus}",
            id, order.Status);

        return Task.FromResult(MapToResponse(order));
    }

    // ── Private helpers ───────────────────────────────────
    private static OrderResponse MapToResponse(Order o) => new(
        o.Id,
        o.CustomerId,
        o.Status,
        o.TotalAmount,
        o.CreatedAt,
        o.Items.Select(i => new OrderItemDto(i.ProductId, i.Name, i.Quantity, i.UnitPrice)).ToList()
    );
}
