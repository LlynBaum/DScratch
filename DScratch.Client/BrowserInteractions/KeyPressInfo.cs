using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace DScratch.Client.BrowserInteractions;

public class KeyPressInfo
{
    public required string InputType { get; init; }
    
    public required string? Data { get; init; }
    
    public required SelectionInfo Selection { get; init; }
    
    public class SelectionInfo
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
            var originOffset = Direction is SelectionDirection.Forward
                ? AnchorNodeId
                : FocusNodeId;
            
            var rightOriginOffset = Direction is SelectionDirection.Forward
                ? FocusNodeId
                : AnchorNodeId;

            return (originOffset, rightOriginOffset);
        }
        
        public (int originOffset, int rightOriginOffset) GetConvertedOffsets()
        {
            var originOffset = Direction is SelectionDirection.Forward
                ? AnchorOffset
                : FocusOffset;
            
            var rightOriginOffset = Direction is SelectionDirection.Forward
                ? FocusOffset
                : AnchorOffset;

            return (originOffset, rightOriginOffset);
        }
    }
}

[JsonConverter(typeof(JsonStringEnumConverter<SelectionDirection>))]
public enum SelectionDirection
{
    [EnumMember(Value = "none")]
    None,
    
    [EnumMember(Value = "backward")]
    Backward,
    
    [EnumMember(Value = "forward")]
    Forward
}
