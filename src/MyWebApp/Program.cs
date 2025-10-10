using MyNuget.Azure.AppConfiguration;
using MyWebApp;

var builder = WebApplication.CreateBuilder(args);

builder.AddMyApi();

builder.AddTodos();
builder.AddMongoTodos();

//builder.Services.AddHostedService<MyBackgroundService>();
builder.Services.AddHttpClient<SelfHttpClient>(client =>
{
    client.BaseAddress = new Uri("https://localhost:5555");
});

var app = builder.Build();

var myAppConfigurationOptions = new MyAppConfigurationOptions();
builder.Configuration.Bind(MyAppConfigurationOptions.SectionName, myAppConfigurationOptions);
if (myAppConfigurationOptions.Endpoint?.IsAbsoluteUri ?? false)
{
    app.UseAzureAppConfiguration();
}

app.MapMyApi();

app.Run();
