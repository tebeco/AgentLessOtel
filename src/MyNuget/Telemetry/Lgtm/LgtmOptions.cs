using OpenTelemetry.Exporter;

namespace MyNuget.Telemetry.Lgtm;

public class LgtmOptions
{
    public const string SectionName = "Lgtm";

    public Uri Endpoint { get; set; } = new("http://localhost:4317/");

    public OtlpExportProtocol Protocol { get; set; } = OtlpExportProtocol.Grpc;
}
