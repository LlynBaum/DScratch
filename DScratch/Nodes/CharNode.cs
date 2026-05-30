namespace DScratch.Nodes;

public class CharNode(char value, string id, CharNode? origin, CharNode? rightOrigin) 
    : DNode(id, origin, rightOrigin)
{
    public char Value { get; } = value;
    
    internal override void InsertChild(DNode node)
    {
        throw new InvalidOperationException("DCharNode has no child nodes.");
    }

    public bool IsWhiteSpace() => char.IsWhiteSpace(Value);
}