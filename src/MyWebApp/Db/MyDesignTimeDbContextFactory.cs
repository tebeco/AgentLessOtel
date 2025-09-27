using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MyWebApp.Db;

public class MyDesignTimeDbContextFactory : IDesignTimeDbContextFactory<MyDbContext>
{
    public MyDbContext CreateDbContext(string[] args)
    {
        var dbContextBuilder = new DbContextOptionsBuilder<MyDbContext>();

        dbContextBuilder.UseNpgsql((builder) =>
        {
            builder.ConfigureDataSource(dataSourceBuilder =>
            {
                dataSourceBuilder.ConnectionStringBuilder.Host = "localhost";
                dataSourceBuilder.ConnectionStringBuilder.Port = 5432;
                dataSourceBuilder.ConnectionStringBuilder.Database = "tododb";
                dataSourceBuilder.ConnectionStringBuilder.Username = "postgres";
                dataSourceBuilder.ConnectionStringBuilder.Password = "postgres";
            });
        });

        return new MyDbContext(dbContextBuilder.Options);
    }
}
