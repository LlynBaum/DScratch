namespace DScratch.Client.Scripts;

public class KeyPressInfo
{
    public required KeyInfo Key { get; init; }
    
    public required string[] Path { get; init; }
    
    public required SelectionInfo Selection { get; init; }
    
    public class KeyInfo
    {
        public required string Value { get; init; }
        
        public bool Alt { get; init; }
        
        public bool Ctrl { get; init; }
        
        public bool Shift { get; init; }
    }
    
    public class SelectionInfo
    {
        public int Offset { get; init; }
        
        public required string Direction { get; init; }
        
        public string[] End { get; init; }
        
        public int EndOffset { get; init; }
    }
}