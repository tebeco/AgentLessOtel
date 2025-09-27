using Microsoft.Extensions.Options;
using OpenTelemetry.Resources;

namespace MyNuget.Telemetry;

// related docs:
// https://docs.datadoghq.com/opentelemetry/config/environment_variable_support/
/*  Datadog convention: DD_TAGS
    Key-value pairs to be used as resource attributes. See Resource semantic conventions for details
    Notes: Only the first 10 key-value pairs are used; the subsequent values are dropped
    deployment.environment and deployment.environment.name map to the DD_ENV environment variable
    service.name maps to the DD_SERVICE environment variable
    service.version maps to the DD_VERSION environment variable
 */
public class MyResourceDetector(IOptions<InfrastructureOptions> options) : IResourceDetector
{
    public const string AttributeTeam = "team";
    public const string AttributeEnvironment = "deployment.environment.name";
    public const string AttributeServiceName = "service.name";
    public const string AttributeServiceVersion = "service.version";

    public Resource Detect()
        => new ([
            new (AttributeTeam, options.Value.Team ),
            new (AttributeEnvironment, options.Value.Environment),
            new (AttributeServiceName, options.Value.Service),
            new (AttributeServiceVersion, options.Value.Version),
        ]);
}
