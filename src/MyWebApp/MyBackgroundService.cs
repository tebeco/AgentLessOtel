using System.Diagnostics;
using MyWebApp.Todos.v1.Dtos;

namespace MyWebApp;

public class MyBackgroundService(IHostEnvironment env, ILogger<MyBackgroundService> _logger, SelfHttpClient _selfHttpClient) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var _activitySource = new ActivitySource(env.ApplicationName);
        while (!stoppingToken.IsCancellationRequested)
        {
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // Set the "activity name" else it will take the "CallerMemberName" which will be "ExecuteAsync"
            using var activity = _activitySource.StartActivity(nameof(MyBackgroundService));
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////

            activity?.SetTag("tag.key", "tag-value");
            activity?.SetTag("tag.rnd", Random.Shared.Next(1, 10));

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

            activity?.Dispose();
            await Task.Delay(TimeSpan.FromSeconds(2), default(CancellationToken));
        }
    }
}
