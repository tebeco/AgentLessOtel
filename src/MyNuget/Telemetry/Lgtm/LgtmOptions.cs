using OpenTelemetry.Exporter;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.Hosting;

public class LgtmOptions
{
    public const string SectionName = "Lgtm";

    public Uri Endpoint { get; set; } = new("http://localhost:4317/");

    public OtlpExportProtocol Protocol { get; set; } = OtlpExportProtocol.Grpc;
}
