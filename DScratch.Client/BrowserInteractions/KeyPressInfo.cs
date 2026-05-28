using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace DScratch.Client.BrowserInteractions;

public class KeyPressInfo
{
    public required string InputType { get; init; }
    
    public required string? Data { get; set; }
    
    public required string[] Path { get; init; }
    
    public required SelectionInfo Selection { get; init; }

    public NodePath GetNodePath() => NodePath.FromJs(Path);
    
    public class SelectionInfo
    {
        public int Offset { get; init; }
        
        public SelectionDirection Direction { get; init; }
        
        public required string[] End { get; init; }
        
        public NodePath GetEnd() => NodePath.FromJs(End);
        
        public int EndOffset { get; init; }
        
        public (int originOffset, int rightOriginOffset) GetConvertedOffsets()
        {
            var originOffset = Direction is SelectionDirection.Forward
                ? Offset
                : EndOffset;
            
            var rightOriginOffset = Direction is SelectionDirection.Forward
                ? EndOffset
                : Offset;

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
