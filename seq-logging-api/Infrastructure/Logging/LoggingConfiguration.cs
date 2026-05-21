// ============================================================
//  Infrastructure/Logging/LoggingConfiguration.cs
//
//  All Serilog wiring lives here — one place to change if you
//  want to add a new sink, enricher, or filter.
// ============================================================

using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

namespace SeqLoggingApi.Infrastructure.Logging;

public static class LoggingConfiguration
{
    /// <summary>
    /// Configures the full Serilog pipeline:
    ///   1. Minimum level overrides per namespace
    ///   2. Enrichers  — metadata added to EVERY log event
    ///   3. Filters    — drop noisy or sensitive events
    ///   4. Sinks      — where logs are written (Console + Seq)
    /// </summary>
    public static void Configure(
        LoggerConfiguration       cfg,
        IConfiguration            configuration,
        IHostEnvironment          env)
    {
        cfg
            // ── Minimum levels ──────────────────────────────────
            // Read base level from appsettings ("Serilog:MinimumLevel").
            .ReadFrom.Configuration(configuration)

            // Silence the most verbose Microsoft/System namespaces.
            // Your own code still logs at the level set in config.
            .MinimumLevel.Override("Microsoft",                   LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.Hosting.Lifetime",  LogEventLevel.Information)
            .MinimumLevel.Override("System",                      LogEventLevel.Warning)

            // ── Enrichers ───────────────────────────────────────
            // Every log event will automatically carry these fields
            // so you can filter/group by them in Seq.

            // Picks up any properties pushed via LogContext.PushProperty(...)
            .Enrich.FromLogContext()

            // Adds: MachineName — useful when running multiple instances
            .Enrich.WithMachineName()

            // Adds: EnvironmentName ("Development" / "Production" / …)
            .Enrich.WithEnvironmentName()

            // Adds: ThreadId — helps correlate concurrent log lines
            .Enrich.WithThreadId()

            // Adds: ProcessId — useful in containerised environments
            .Enrich.WithProcessId()

            // Adds: TraceId + SpanId from System.Diagnostics.Activity
            // This links your log events to distributed traces.
            .Enrich.WithSpan()

            // A static property on every event — great for dashboards
            .Enrich.WithProperty("Application", env.ApplicationName)
            .Enrich.WithProperty("Environment",  env.EnvironmentName)

            // ── Filters ─────────────────────────────────────────
            // Drop health-check endpoint noise from the log stream.
            .Filter.ByExcluding(le =>
                le.Properties.TryGetValue("RequestPath", out var v) &&
                v.ToString().Contains("/health"))

            // ── Sinks ───────────────────────────────────────────

            // Console sink:
            //   Development → human-readable coloured output
            //   Production  → compact JSON (one line per event, easy to parse)
            .WriteTo.Conditional(
                _ => env.IsDevelopment(),
                wt => wt.Console(
                    outputTemplate:
                        "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext}\n" +
                        "  {Message:lj}{NewLine}{Exception}"))
            .WriteTo.Conditional(
                _ => !env.IsDevelopment(),
                wt => wt.Console(new CompactJsonFormatter()))

            // Seq sink:
            //   serverUrl  — read from config (never hard-code in source)
            //   apiKey     — optional; set in Seq server for auth
            //   bufferPath — durable disk buffer; events survive a Seq outage
            //   batchSize  — how many events to send per HTTP POST (_bulk)
            //   period     — how often to flush the buffer to Seq
            .WriteTo.Seq(
                serverUrl:   configuration["Seq:ServerUrl"]  ?? "http://localhost:5341",
                apiKey:      configuration["Seq:ApiKey"],
                bufferPath:  "./logs/seq-buffer",
                batchPostingLimit: 500,
                period:      TimeSpan.FromSeconds(2));
    }
}
