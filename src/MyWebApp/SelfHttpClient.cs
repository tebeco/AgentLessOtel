namespace MyWebApp;

public class SelfHttpClient(HttpClient httpClient)
{
    public async Task<string> GetRootAsync()
    {
        return await httpClient.GetStringAsync("/");
    }

    public async Task<string> GetReadinesstAsync()
    {
        return await httpClient.GetStringAsync("/health");
    }

    public async Task<string> GetLivenesstAsync()
    {
        return await httpClient.GetStringAsync("/live");
    }
}
