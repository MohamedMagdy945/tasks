// ============================================================
//  Features/Orders/OrdersController.cs
//
//  REST controller for the Orders resource.
//  Demonstrates:
//   • Injecting ILogger<T> the standard way
//   • Using the high-perf LoggerMessage methods
//   • Letting the service layer do the heavy-log lifting
//   • A deliberate "simulate error" endpoint for testing Seq
// ============================================================

using Microsoft.AspNetCore.Mvc;
using SeqLoggingApi.Infrastructure.Logging;
using SeqLoggingApi.Models;

namespace SeqLoggingApi.Features.Orders;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public sealed class OrdersController(
    IOrderService              orderService,
    ILogger<OrdersController>  logger) : ControllerBase
{
    // GET api/orders
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<OrderResponse>), 200)]
    public async Task<IActionResult> GetAll()
    {
        logger.LogDebug("GET /api/orders — fetching all orders");

        var orders = await orderService.GetAllAsync();
        return Ok(orders);
    }

    // GET api/orders/{id}
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(OrderResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(int id)
    {
        // The service layer logs RetrievingOrder + OrderNotFound,
        // so we only need a controller-level trace here.
        logger.LogDebug("GET /api/orders/{OrderId}", id);

        var order = await orderService.GetByIdAsync(id);
        return Ok(order);
    }

    // POST api/orders
    [HttpPost]
    [ProducesResponseType(typeof(OrderResponse), 201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Create([FromBody] CreateOrderRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errors = string.Join("; ", ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage));

            ApiLogMessages.ValidationFailed(logger, "POST /api/orders", errors);
            return BadRequest(ModelState);
        }

        var order = await orderService.CreateAsync(request);

        // 201 Created with Location header pointing to the new resource.
        return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
    }

    // DELETE api/orders/{id}/cancel
    [HttpDelete("{id:int}/cancel")]
    [ProducesResponseType(typeof(OrderResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Cancel(int id, [FromQuery] string reason = "Cancelled by user")
    {
        var order = await orderService.CancelAsync(id, reason);
        return Ok(order);
    }

    // ── Diagnostic / demo endpoints ───────────────────────

    // GET api/orders/simulate-error
    // Throws an intentional exception so you can see how
    // GlobalExceptionMiddleware logs it in Seq.
    [HttpGet("simulate-error")]
    public IActionResult SimulateError()
    {
        logger.LogWarning("Simulating an unhandled exception for demonstration …");
        throw new InvalidOperationException(
            "This is a deliberate exception — check Seq to see it logged!");
    }

    // GET api/orders/simulate-levels
    // Fires one log event at each level so you can see them all in Seq.
    [HttpGet("simulate-levels")]
    public IActionResult SimulateLevels()
    {
        logger.LogTrace("TRACE — ultra-verbose, usually disabled");
        logger.LogDebug("DEBUG — useful during development");
        logger.LogInformation("INFO  — normal operational message");
        logger.LogWarning("WARN  — something unexpected but non-fatal");
        logger.LogError(new Exception("demo error"), "ERROR — something failed");
        logger.LogCritical("CRIT  — system-level failure");

        return Ok(new { message = "All log levels fired — check Seq!" });
    }
}
