namespace DScratch.Client.BrowserInteractions.Metadata;

public class MetadataEntry(IMetadata data)
{
    public MetadataDelete MetadataDelete => Data.MetadataDelete;

    public IMetadata Data { get; } = data;
}