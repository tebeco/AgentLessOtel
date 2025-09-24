using AgentLessOtelDataDog;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Net.Mime;

var builder = WebApplication.CreateBuilder(args);

// Minimal repro code for testing
// * endpoint (traces + log correlation)
// * external http call (external traces correlation)
// * background service (traces + external traces + log correlation)
builder.Services.AddHealthChecks().AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);
builder.Services.AddHostedService<MyBackgroundService>();
builder.Services.AddHttpClient<SelfHttpClient>(client =>
{
    client.BaseAddress = new Uri("https://localhost:5555");
});

// DATADOG SPECIFIC INTEGRATION
builder.Services
    .AddOptionsWithValidateOnStart<DatadogOptions>()
    .BindConfiguration(DatadogOptions.SectionName)
    .Validate(options => !string.IsNullOrWhiteSpace(options.ApiKey), """IF YOU SEE THIS ERROR YOU NEED TO RUN 'dotnet user-secrets --id agentless-otel-datadog set "Datadog:ApiKey" "<YOUR API KEY GOES HERE>"'"""); // 

builder.AddOpenTelemetry();

var app = builder.Build();

app.MapGet("/", async ([FromServices] SelfHttpClient selfHttpClient) =>
{
    var livenessResult = await selfHttpClient.GetLivenesstAsync();
    var readinessResult = await selfHttpClient.GetReadinesstAsync();
    return TypedResults.Text($"""
        <ul>
            <li>livenessResult: {livenessResult}</li>
            <li>readinessResult: {readinessResult}</li>
        </ul>
        """, MediaTypeNames.Text.Html);
});
app.MapHealthChecks("/health");
app.MapHealthChecks("/live", new HealthCheckOptions { Predicate = r => r.Tags.Contains("live") });

app.Run();
