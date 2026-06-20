namespace DScratch.Interactions;

public class KeyPressInfo
{
    public required string InputType { get; init; }
    
    public required string? Data { get; init; }
    
    public required SelectionInfo Selection { get; init; }
}