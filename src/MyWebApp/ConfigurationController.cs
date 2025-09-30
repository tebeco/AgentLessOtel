using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;

namespace MyWebApp;

[ApiController]
[Route("/config")]
public class ConfigurationController(
    IConfiguration configuration,
    IConfigurationRefresherProvider refresherProvider) : ControllerBase
{
    [HttpGet]
    public Ok<string> Get([FromQuery(Name = "key")] string key)
        => TypedResults.Ok(configuration.GetValue<string>(key) ?? "(null)");

    [HttpPost("refresh")]
    public async Task<NoContent> Refresh()
    {
        foreach(var refresher in refresherProvider.Refreshers)
        {
            await refresher.TryRefreshAsync();
        };
        return TypedResults.NoContent();
    }
}
