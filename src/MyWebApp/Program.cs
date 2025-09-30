using MyWebApp;

var builder = WebApplication.CreateBuilder(args);

builder.AddMyApi();

builder.Services.AddTodos();
builder.Services.AddMongoTodos();

builder.Services.AddHostedService<MyBackgroundService>();
builder.Services.AddHttpClient<SelfHttpClient>(client =>
{
    client.BaseAddress = new Uri("https://localhost:5555");
});

var app = builder.Build();

app.UseAzureAppConfiguration();

app.MapMyApi();

app.Run();
