namespace DScratch.Client.Scripts;

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
        
        public required string Direction { get; init; }
        
        public required string[] End { get; init; }
        
        public NodePath GetEnd() => NodePath.FromJs(End);
        
        public int EndOffset { get; init; }
    }
}