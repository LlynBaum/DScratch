namespace DScratch.Interactions;

public record SelectionInfo
{
    public SelectionDirection Direction { get; init; }

    public required string AnchorId { get; init; }

    public NodeId AnchorNodeId => NodeId.FromString(AnchorId);
        
    public int AnchorOffset { get; init; }
        
    public required string FocusId { get; init; }

    public NodeId FocusNodeId => NodeId.FromString(FocusId);
        
    public int FocusOffset { get; init; }
        
    public (NodeId origin, NodeId rightOrigin) GetConvertedNodeIds()
    {
        var originOffset = Direction is SelectionDirection.Forward or SelectionDirection.None
            ? AnchorNodeId
            : FocusNodeId;
            
        var rightOriginOffset = Direction is SelectionDirection.Forward or SelectionDirection.None
            ? FocusNodeId
            : AnchorNodeId;

        return (originOffset, rightOriginOffset);
    }
        
    public (int originOffset, int rightOriginOffset) GetConvertedOffsets()
    {
        var originOffset = Direction is SelectionDirection.Forward or SelectionDirection.None
            ? AnchorOffset
            : FocusOffset;
            
        var rightOriginOffset = Direction is SelectionDirection.Forward or SelectionDirection.None
            ? FocusOffset
            : AnchorOffset;

        return (originOffset, rightOriginOffset);
    }
}