#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;

public class MyMongoDbOptions
{
    public const string SectionName = "MongoDb";

    public required string ConnectionString { get; set; } = "mongodb://localhost:27017";

    public required string DatabaseName { get; set; } = "tododb";
}
