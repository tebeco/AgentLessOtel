using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyNuget.EfCore;
using Npgsql;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class MyDbContextServiceCollectionExtensions
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // used in startup because specific ActivitySource needs to be specifically "Added" into the .WithTracing
    public const string MyActivitySourceName = "MyNuget.EfCore.ActivitySource";
    public static readonly ActivitySource ActivitySource = new(MyActivitySourceName);
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public static IHostApplicationBuilder AddMyDbContext<TDbContext>(this IHostApplicationBuilder builder)
        where TDbContext : DbContext
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // WORKS BUT RELY ON EXPLICIT CONNECTION STRING being "null!".
        builder.Services.AddNpgsqlDataSource(
                   connectionString: null!,
                   (sp, dataSourceBuilder) =>
                   {
                       dataSourceBuilder.UseLoggerFactory(sp.GetRequiredService<ILoggerFactory>());
                       dataSourceBuilder.ConnectionStringBuilder.Host = "localhost";
                       dataSourceBuilder.ConnectionStringBuilder.Port = 5432;
                       dataSourceBuilder.ConnectionStringBuilder.Database = "tododb";
                       dataSourceBuilder.ConnectionStringBuilder.Username = "postgres";
                       dataSourceBuilder.ConnectionStringBuilder.Password = "postgres";
                   });

        builder.Services.AddDbContext<TDbContext>((sp, options) =>
        {
            options.EnableSensitiveDataLogging();
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

            var npgsqlDataSource = sp.GetRequiredService<NpgsqlDataSource>();

            options.UseNpgsql(
                npgsqlDataSource,
                o =>
                {
                    o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                });
        });

        builder.Services.AddOpenTelemetry()
            .WithTracing(tracing =>
                tracing
                    .AddSource(MyActivitySourceName)
                    .AddNpgsql())
            .WithMetrics(metrics => metrics.AddNpgsqlInstrumentation());

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //services.AddDbContext<TDbContext>((sp, options) =>
        //{
        //    options.UseNpgsql(optionsBuider =>
        //    {
        //        options.EnableSensitiveDataLogging();
        //        options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

        //        optionsBuider.ConfigureDataSource(dataSourceBuilder =>
        //        {
        //            dataSourceBuilder.UseLoggerFactory(sp.GetRequiredService<ILoggerFactory>());
        //            dataSourceBuilder.ConnectionStringBuilder.Host = "localhost";
        //            dataSourceBuilder.ConnectionStringBuilder.Port = 5432;
        //            dataSourceBuilder.ConnectionStringBuilder.Database = "tododb";
        //            dataSourceBuilder.ConnectionStringBuilder.Username = "postgres";
        //            dataSourceBuilder.ConnectionStringBuilder.Password = "postgres";
        //        });
        //    });
        //});
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        builder.Services.AddHostedService<MyDbContextInitializerHostedService<TDbContext>>();

        return builder;
    }
}
