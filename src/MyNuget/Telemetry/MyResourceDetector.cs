using Microsoft.Extensions.Options;
using OpenTelemetry.Resources;

namespace MyNuget.Telemetry;

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
