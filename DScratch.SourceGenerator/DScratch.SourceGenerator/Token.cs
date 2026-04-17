namespace DScratch.SourceGenerator;

public struct Token
{
    public TokenType Type { get; set; }
        
    public string Text { get; set; }
        
    public object? Literal { get; set; }
}