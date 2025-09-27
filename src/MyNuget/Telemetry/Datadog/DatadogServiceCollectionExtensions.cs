using Microsoft.Extensions.DependencyInjection;
using MyNuget.Telemetry.Datadog;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.Hosting;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class DatadogServiceCollectionExtensions
{
    public static IHostApplicationBuilder AddDatadogOptions(this IHostApplicationBuilder builder)
    {
        builder.Services
            .AddOptionsWithValidateOnStart<DatadogOptions>()
            .BindConfiguration(DatadogOptions.SectionName)
            .Validate(options => !string.IsNullOrWhiteSpace(options.ApiKey), """IF YOU SEE THIS ERROR YOU NEED TO RUN 'dotnet user-secrets --id agentless-otel-datadog set "Datadog:ApiKey" "<YOUR API KEY GOES HERE>"'"""); // 

        return builder;
    }
}