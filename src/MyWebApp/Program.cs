using MyWebApp;
using MyWebApp.Db;
using MyWebApp.Todos;

var builder = WebApplication.CreateBuilder(args);

builder.AddMyApi();

builder.Services.AddTransient<TodoService>();
builder.Services.AddMyDbContext<MyDbContext>();

builder.Services.AddHostedService<MyBackgroundService>();
builder.Services.AddHttpClient<SelfHttpClient>(client =>
{
    client.BaseAddress = new Uri("https://localhost:5555");
});

var app = builder.Build();

app.MapMyApi();

app.Run();
