using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHealthChecks().AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);

var app = builder.Build();

app.MapHealthChecks("/", new HealthCheckOptions
{
    Predicate = r => r.Tags.Contains("live")
});

app.Run();
