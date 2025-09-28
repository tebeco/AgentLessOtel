using MyWebApp;
using MyWebApp.Db;
using MyWebApp.Todos;
using MyWebApp.MongoTodos.Entities;

var builder = WebApplication.CreateBuilder(args);

builder.AddMyApi();

builder.Services.AddTransient<TodoService>();
builder.Services.AddMyDbContext<MyDbContext>();

builder.Services.AddMyMongoDb();
builder.Services.AddMongoCollection<Todo>();

builder.Services.AddHostedService<MyBackgroundService>();
builder.Services.AddHttpClient<SelfHttpClient>(client =>
{
    client.BaseAddress = new Uri("https://localhost:5555");
});

var app = builder.Build();

app.MapMyApi();

app.Run();
