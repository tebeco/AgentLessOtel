
using MyWebApp.Todos.v1.Dtos;

namespace MyWebApp;

public class SelfHttpClient(HttpClient httpClient)
{
    public HttpClient HttpClient => httpClient;

    public async Task<string> GetRootAsync() => await httpClient.GetStringAsync("/");

    public async Task<string> GetReadinesstAsync() => await httpClient.GetStringAsync("/health");

    public async Task<string> GetLivenesstAsync() => await httpClient.GetStringAsync("/live");

    public async Task<string> GetTodosAsync() => await httpClient.GetStringAsync("/api/1.0/todos");

    public async Task<string> GetTodoAsync(int id) => await httpClient.GetStringAsync($"/api/1.0/todos/{id}");

    public async Task<Uri?> CreateTodoAsync(CreateTodoDto createTodoDto)
    {
        var response = await httpClient.PostAsJsonAsync("/api/1.0/todos", createTodoDto);
        return response.Headers.Location;
    }
}
