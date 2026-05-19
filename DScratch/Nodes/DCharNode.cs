namespace DScratch.Nodes;

public class DCharNode(char value, string id, DCharNode? origin, DCharNode? rightOrigin, DNode? parent) 
    : DNode(id, origin, rightOrigin, parent, null)
{
    public override string TagName => throw new InvalidOperationException();
    
    public char Value { get; } = value;

    public DCharNode? NextChar => (DCharNode?)RightOrigin;
    
    internal override void InsertChild(DNode node)
    {
        throw new InvalidOperationException("DCharNode has no child nodes.");
    }

    internal override void InsertChildRange(DNode first, DNode last)
    {
        throw new InvalidOperationException("DCharNode has no child nodes.");
    }

    internal override void DeleteChild(string id)
    {
        throw new InvalidOperationException("DCharNode has no child nodes.");
    }
}