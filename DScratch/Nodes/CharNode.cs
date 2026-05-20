namespace DScratch.Nodes;

public class CharNode(char value, string id, CharNode? origin, CharNode? rightOrigin, DNode? parent) 
    : DNode(id, origin, rightOrigin, parent)
{
    public char Value { get; } = value;

    public CharNode? NextChar => (CharNode?)RightOrigin;
    
    internal override void InsertChild(DNode node)
    {
        throw new InvalidOperationException("DCharNode has no child nodes.");
    }
}