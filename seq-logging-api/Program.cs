// ============================================================
//  Program.cs — Entry point
//  Sets up Serilog with the Seq sink BEFORE the DI container
//  is built, so startup errors are also captured.
// ============================================================

using Serilog;
using Serilog.Events;
using SeqLoggingApi.Infrastructure.Logging;
using SeqLoggingApi.Infrastructure.Middleware;

// ── Step 1: Bootstrap logger ────────────────────────────────
// A minimal logger active from the very first line of code.
// It only writes to the Console so we can see startup errors
// before the real Serilog pipeline is ready.
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting SeqLoggingApi …");

    var builder = WebApplication.CreateBuilder(args);

    // ── Step 2: Replace the bootstrap logger with the full one ─
    // UseSerilog() with a callback gives us access to the fully
    // built IConfiguration and IServiceProvider at this point.
    builder.Host.UseSerilog((ctx, services, cfg) =>
        LoggingConfiguration.Configure(cfg, ctx.Configuration, ctx.HostingEnvironment));

    // ── Step 3: Register application services ──────────────────
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new() { Title = "Seq Logging Demo API", Version = "v1" });
    });

    // Register our demo services
    builder.Services.AddScoped<SeqLoggingApi.Features.Orders.IOrderService,
                               SeqLoggingApi.Features.Orders.OrderService>();

    // ── Step 4: Build & configure the HTTP pipeline ────────────
    var app = builder.Build();

    // Global exception handler — catches unhandled exceptions,
    // logs them with full context, and returns a clean JSON error.
    app.UseMiddleware<GlobalExceptionMiddleware>();

    // Serilog request logging middleware — logs one structured
    // entry per HTTP request (method, path, status, duration).
    app.UseSerilogRequestLogging(opts =>
    {
        opts.MessageTemplate =
            "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";

        // Enrich the request log with extra fields visible in Seq.
        opts.EnrichDiagnosticContext = (diag, http) =>
        {
            diag.Set("RequestHost",   http.Request.Host.Value);
            diag.Set("RequestScheme", http.Request.Scheme);
            diag.Set("UserAgent",     http.Request.Headers.UserAgent.ToString());
            if (http.User.Identity?.IsAuthenticated == true)
                diag.Set("UserId", http.User.FindFirst("sub")?.Value ?? "unknown");
        };
    });

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseRouting();
    app.MapControllers();

    app.Run();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    // Any exception that prevents startup is logged as Fatal.
    Log.Fatal(ex, "Application terminated unexpectedly during startup");
}
finally
{
    // IMPORTANT: flush all buffered log events before the process exits.
    // Without this, the last few events may never reach Seq.
    await Log.CloseAndFlushAsync();
}
