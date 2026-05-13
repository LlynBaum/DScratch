namespace DScratch;

public class CharNode(char value, string id, CharNode? origin, CharNode? rightOrigin) : DNode(id, origin, rightOrigin)
{
    public char Value { get; } = value;

    public CharNode? NextChar => (CharNode?)RightOrigin;
}