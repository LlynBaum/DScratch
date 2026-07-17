using DScratch.Nodes.Marks;

namespace DScratch.Client.BrowserInteractions.Metadata;

public class PositionMetadata(Mark mark) : IMetadata
{
    public MetadataDelete MetadataDelete => MetadataDelete.OnSelectionChange;
    
    public Mark Mark { get; } = mark;
}