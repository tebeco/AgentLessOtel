using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;
using MongoDB.Driver.Core.Extensions.DiagnosticSources;
using MyNuget.Mongo;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.Hosting;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class MongoServiceCollectionExtensions
{
    private const string ActivityNameSource = "MongoDB.Driver.Core.Extensions.DiagnosticSources";

    public static IHostApplicationBuilder AddMyMongoDb(this IHostApplicationBuilder builder)
    {
        builder
            .Services
            .AddOptionsWithValidateOnStart<MyMongoDbOptions>()
            .BindConfiguration(MyMongoDbOptions.SectionName);

        builder
            .Services
            .AddSingleton<IMongoClient>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<MyMongoDbOptions>>().Value;

                var instrumentationOptions = new InstrumentationOptions { CaptureCommandText = true };
                var diagnosticsActivityEventSubscriber = new DiagnosticsActivityEventSubscriber(instrumentationOptions);

                var settings = MongoClientSettings.FromConnectionString(options.ConnectionString);
                settings.LoggingSettings = new LoggingSettings(sp.GetRequiredService<ILoggerFactory>());
                settings.ClusterConfigurator = cb => cb.Subscribe(diagnosticsActivityEventSubscriber);

                return new MongoClient(settings);
            });

        builder
            .Services
            .AddSingleton<IMongoDatabase>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<MyMongoDbOptions>>().Value;
                var mongoClient = sp.GetRequiredService<IMongoClient>();

                return mongoClient.GetDatabase(options.DatabaseName);
            });

        builder
            .Services
            .AddOpenTelemetry()
            .WithTracing(tracer => tracer.AddSource(DiagnosticsActivityEventSubscriber.ActivitySourceName));

        return builder;
    }

    public static IServiceCollection AddMongoCollection<T>(this IServiceCollection services, string collectionName)
        where T : notnull
    {
        services.AddSingleton(sp =>
        {
            var mongoDatabase = sp.GetRequiredService<IMongoDatabase>();

            return mongoDatabase.GetCollection<T>(collectionName);
        });

        return services;
    }
}
