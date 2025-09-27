using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MyWebApp;
using MyWebApp.Db;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.AddOpenTelemetry();

builder.Services.AddAgileDbContext();

builder.Services.AddHealthChecks().AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);
builder.Services.AddHostedService<MyBackgroundService>();
builder.Services.AddHttpClient<SelfHttpClient>(client =>
{
    client.BaseAddress = new Uri("https://localhost:5555");
});

var app = builder.Build();

app.MapOpenApi();
app.UseHttpsRedirection();
app.UseAuthorization();

app.MapHealthChecks("/live", new HealthCheckOptions { Predicate = r => r.Tags.Contains("live") });

app.MapControllers();

app.Run();
