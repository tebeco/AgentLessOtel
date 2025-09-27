using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Scalar.AspNetCore;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.AspNetCore.Routing;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class MyApiEndpointRouteBuilderExtensions
{
    public static WebApplication MapMyApi(this WebApplication app)
    {
        app.MapOpenApi();

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapHealthChecks("/health");
        app.MapHealthChecks("/live", new HealthCheckOptions { Predicate = r => r.Tags.Contains("live") });

        app.MapScalarApiReference();

        app.MapControllers();

        return app;
    }
}
