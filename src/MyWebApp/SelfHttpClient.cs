namespace MyWebApp;

public class SelfHttpClient(HttpClient httpClient)
{
    public async Task<string> GetRootAsync() => await httpClient.GetStringAsync("/");

    public async Task<string> GetReadinesstAsync() => await httpClient.GetStringAsync("/health");

    public async Task<string> GetLivenesstAsync() => await httpClient.GetStringAsync("/live");
}
