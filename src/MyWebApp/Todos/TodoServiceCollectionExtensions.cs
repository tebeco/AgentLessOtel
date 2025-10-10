using MyWebApp.Db;
using MyWebApp.Todos;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class TodoServiceCollectionExtensions
{
    public static IHostApplicationBuilder AddTodos(this IHostApplicationBuilder builder)
    {
        builder.AddMyDbContext<MyDbContext>();
        builder.Services.AddTransient<TodoService>();

        return builder;
    }
}
