namespace DScratch.Nodes;

public class DCharNode(char value, string id, DCharNode? origin, DCharNode? rightOrigin) : DNode(id, origin, rightOrigin, null)
{
    public char Value { get; } = value;

    public DCharNode? NextChar => (DCharNode?)RightOrigin;
}