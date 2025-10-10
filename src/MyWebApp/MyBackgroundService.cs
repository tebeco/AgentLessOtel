using System.Diagnostics;
using MyWebApp.Todos.v1.Dtos;

namespace MyWebApp;

public class MyBackgroundService(ILogger<MyBackgroundService> _logger, SelfHttpClient _selfHttpClient) : BackgroundService
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // used in startup because specific ActivitySource needs to be specifically "Added" into the .WithTracing
    public const string MyActivitySourceName = "MyWebApp.ActivitySource";
    private static readonly ActivitySource _activitySource = new(MyActivitySourceName);
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // Set the "activity name" else it will take the "CallerMemberName" which will be "ExecuteAsync"
            using var activity = _activitySource.StartActivity(nameof(MyBackgroundService));
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // POSSIBLE BUG HERE
            // Attempt to add a "Baggage" to the trace, but datadog doesn't reflect that yet
            activity?.SetTag("baggage.key", "baggage-value");
            activity?.SetTag("baggage.rnd", Random.Shared.Next(1, 10));
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // make sure log/trace correlation properly work with background task
            _logger.LogInformation("MyBackgroundService is running at: {time}", DateTimeOffset.Now);
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // make sure background traces properly work with background task
            await _selfHttpClient.GetRootAsync();
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // make sure background traces properly work with background task
            var createdTodoUri = await _selfHttpClient.CreateTodoAsync(
                new CreateTodoDto("salfkjhasldfk", false)
            );

            var createdTodo = await _selfHttpClient.HttpClient.GetFromJsonAsync<TodoDto>(createdTodoUri!.AbsolutePath);
            _ = await _selfHttpClient.HttpClient.GetFromJsonAsync<TodoDto>(createdTodoUri.AbsolutePath);
            _ = await _selfHttpClient.HttpClient.GetFromJsonAsync<TodoDto>(createdTodoUri.AbsolutePath);
            _ = await _selfHttpClient.HttpClient.GetFromJsonAsync<TodoDto>(createdTodoUri.AbsolutePath);
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////

            // redis
            // mongo
            // efcore
            // azure
            //  storage account
            //  sb
            //  event hub
            //  function

            activity?.Dispose();
            await Task.Delay(TimeSpan.FromSeconds(2), default(CancellationToken));
        }
    }
}
