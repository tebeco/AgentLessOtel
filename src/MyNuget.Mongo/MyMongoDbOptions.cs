namespace MyNuget.Mongo;

public class MyMongoDbOptions
{
    public const string SectionName = "MongoDb";

    public required string ConnectionString { get; set; } = "mongodb://localhost:27017";

    public required string DatabaseName { get; set; } = "tododb";
}
