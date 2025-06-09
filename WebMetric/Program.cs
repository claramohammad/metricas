using System.Diagnostics.Metrics;
using OpenTelemetry.Metrics;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenTelemetry()
    .WithMetrics(mb =>
    {
        mb.AddMeter("WebMetric.Custom");             // <- nosso Meter
        mb.AddMeter("Microsoft.AspNetCore.Hosting"); // já existente
        mb.AddMeter("Microsoft.AspNetCore.Server.Kestrel");
        mb.AddPrometheusExporter();                  // exporter deve vir depois dos AddMeter
        mb.AddView("http.server.request.duration",
            new ExplicitBucketHistogramConfiguration
            {
                Boundaries = new double[] { /* ... */ }
            });
    });

// Custom metrics
var meter = new Meter("WebMetric.Custom", "1.0.0");
var requestCounter = meter.CreateCounter<long>("webmetric_requests_total",
    description: "Total de requisições recebidas");
builder.Services.AddSingleton(meter); 

var app = builder.Build();

// expõe /metrics para o Prometheus “scrape”
app.MapPrometheusScrapingEndpoint();

app.MapGet("/", () =>
{
    requestCounter.Add(1);
    return "Hello OpenTelemetry! ticks:" + DateTime.Now.Ticks.ToString()[^3..];
});

app.Run();
