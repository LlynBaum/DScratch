using DScratch.Nodes.Marks;

namespace DScratch.Client.BrowserInteractions.Metadata;

public class BlockMetadata(NodeId id, Mark mark) : IMetadata
{
    public MetadataDelete MetadataDelete => MetadataDelete.OnTyping;
    
    public string Id { get; } = id.Value;

    public Mark Mark { get; } = mark;
}