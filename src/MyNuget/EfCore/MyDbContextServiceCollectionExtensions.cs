using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyNuget.EfCore;
using Npgsql;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class MyDbContextServiceCollectionExtensions
{
    public static IServiceCollection AddMyDbContext<TDbContext>(this IServiceCollection services)
        where TDbContext : DbContext
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // WORKS BUT RELY ON EXPLICIT CONNECTION STRING being "null!".
        services.AddNpgsqlDataSource(
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

        services.AddDbContext<TDbContext>((sp, options) =>
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

        services.AddOpenTelemetry()
            .WithTracing(builder => builder.AddNpgsql())
            .WithMetrics(builder => builder.AddNpgsqlInstrumentation());
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

        services.AddHostedService<MyDbContextInitializerHostedService<TDbContext>>();

        return services;
    }
}
