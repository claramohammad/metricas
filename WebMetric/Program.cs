using OpenTelemetry.Metrics;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenTelemetry()
    .WithMetrics(meterProviderBuilder =>
    {
        meterProviderBuilder
          .AddPrometheusExporter()
          .AddMeter("Microsoft.AspNetCore.Hosting", "Microsoft.AspNetCore.Server.Kestrel")
          .AddView("http.server.request.duration",
              new ExplicitBucketHistogramConfiguration
              {
                  Boundaries = new double[] { 0, 0.005, 0.01, 0.025, 0.05,
                      0.075, 0.1, 0.25, 0.5, 0.75, 1, 2.5, 5, 7.5, 10 }
              });
    });

var app = builder.Build();

// expõe /metrics para o Prometheus “scrape”
app.MapPrometheusScrapingEndpoint();

app.MapGet("/", () => "Hello OpenTelemetry! ticks:" + DateTime.Now.Ticks.ToString()[^3..]);

app.Run();
