using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyWebApp.Db;

namespace MyWebApp.Db;

public static class AgileDbContextServiceCollectionExtensions
{
    public static IServiceCollection AddAgileDbContext(this IServiceCollection services)
    {
        services.AddDbContext<AgileDbContext>((sp, options) =>
        {
            options.UseNpgsql((builder) =>
            {
                builder.ConfigureDataSource(dataSourceBuilder =>
                {
                    dataSourceBuilder.UseLoggerFactory(sp.GetRequiredService<ILoggerFactory>());

                    dataSourceBuilder.ConnectionStringBuilder.Host = "localhost";
                    dataSourceBuilder.ConnectionStringBuilder.Port = 5432;
                    dataSourceBuilder.ConnectionStringBuilder.Database = "agiledb";
                    dataSourceBuilder.ConnectionStringBuilder.Username = "postgres";
                    dataSourceBuilder.ConnectionStringBuilder.Password = "postgres";
                });
            });
        });

        services.AddHostedService<AgileDbContextInitializerHostedService>();

        return services;
    }
}
