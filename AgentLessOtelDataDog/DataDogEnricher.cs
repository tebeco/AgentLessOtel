using Microsoft.Extensions.Diagnostics.Enrichment;
using Microsoft.Extensions.Options;

namespace AgentLessOtelDataDog;

// https://andrewlock.net/customising-the-new-telemetry-logging-source-generator/

////////////////////////////////////////////////////////////////////////////////////////////////////////////
// this is a terrible idea as:
// * it only enrich logs
// * it doesn't works for traces
// * it doesn't works for metrics
////////////////////////////////////////////////////////////////////////////////////////////////////////////

public class DataDogEnricher(IOptions<DatadogOptions> options) : IStaticLogEnricher
{
    public void Enrich(IEnrichmentTagCollector collector)
    {
        collector.Add("dd.env", options.Value.Environment);
        collector.Add("dd.service", options.Value.Service);
        collector.Add("dd.version", options.Value.Version);
    }
}
