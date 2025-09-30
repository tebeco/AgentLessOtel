using MyWebApp.MongoTodos;
using MyWebApp.MongoTodos.Entities;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class MongoTodoServiceCollectionExtensions
{
    public static IServiceCollection AddMongoTodos(this IServiceCollection services)
    {
        services.AddMyMongoDb();
        services.AddMongoCollection<Todo>("Todos");
        services.AddTransient<MongoTodoService>();

        return services;
    }
}
