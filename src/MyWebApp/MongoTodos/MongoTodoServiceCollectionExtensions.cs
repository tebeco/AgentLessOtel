using MyWebApp.MongoTodos;
using MyWebApp.MongoTodos.Entities;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class MongoTodoServiceCollectionExtensions
{
    public static IHostApplicationBuilder AddMongoTodos(this IHostApplicationBuilder builder)
    {
        builder.AddMyMongoDb();
        builder.Services.AddMongoCollection<Todo>("Todos");
        builder.Services.AddTransient<MongoTodoService>();

        return builder;
    }
}
