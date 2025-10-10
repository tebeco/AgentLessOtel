using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;
using MyNuget.Mongo;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class MongoServiceCollectionExtensions
{
    private const string ActivityNameSource = "MongoDB.Driver.Core.Extensions.DiagnosticSources";

    public static IServiceCollection AddMyMongoDb(this IServiceCollection services)
    {
        services
            .AddOptionsWithValidateOnStart<MyMongoDbOptions>()
            .BindConfiguration(MyMongoDbOptions.SectionName);

        services.AddSingleton<IMongoClient>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<MyMongoDbOptions>>().Value;

            var settings = MongoClientSettings.FromConnectionString(options.ConnectionString);
            settings.LoggingSettings = new LoggingSettings(sp.GetRequiredService<ILoggerFactory>());

            return new MongoClient(settings);
        });


        services
            .AddOpenTelemetry()
            .WithTracing(tracer => tracer.AddSource(ActivityNameSource));

        return services;
    }

    public static IServiceCollection AddMongoCollection<T>(this IServiceCollection services, string collectionName)
        where T : notnull
    {
        services.AddSingleton(sp =>
        {
            var options = sp.GetRequiredService<IOptions<MyMongoDbOptions>>().Value;
            var mongoClient = sp.GetRequiredService<IMongoClient>();
            var database = mongoClient.GetDatabase(options.DatabaseName);

            return database.GetCollection<T>(collectionName);
        });

        return services;
    }
}
