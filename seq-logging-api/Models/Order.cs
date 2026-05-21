// ============================================================
//  Models/Order.cs  &  DTOs
// ============================================================

namespace SeqLoggingApi.Models;

public sealed class Order
{
    public int      Id           { get; init; }
    public string   CustomerId   { get; init; } = default!;
    public string   Status       { get; set; }  = "Pending";
    public decimal  TotalAmount  { get; init; }
    public DateTime CreatedAt    { get; init; } = DateTime.UtcNow;
    public List<OrderItem> Items { get; init; } = [];
}

public sealed class OrderItem
{
    public int     ProductId { get; init; }
    public string  Name      { get; init; } = default!;
    public int     Quantity  { get; init; }
    public decimal UnitPrice { get; init; }
}

// ── Request / Response DTOs ───────────────────────────────

public sealed record CreateOrderRequest(
    string           CustomerId,
    List<OrderItemDto> Items);

public sealed record OrderItemDto(
    int     ProductId,
    string  Name,
    int     Quantity,
    decimal UnitPrice);

public sealed record OrderResponse(
    int      Id,
    string   CustomerId,
    string   Status,
    decimal  TotalAmount,
    DateTime CreatedAt,
    List<OrderItemDto> Items);
