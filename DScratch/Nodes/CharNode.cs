namespace DScratch.Nodes;

// TODO: remove this and replace with "Continuous Runs" within the text node. Might as well rework the ID generation and add a lookup table for nodes. Transaction must add the nodes to the lookup table.
public class CharNode(char value, NodeId id, CharNode? origin, CharNode? rightOrigin) 
    : DNode(id, origin, rightOrigin)
{
    public char Value { get; } = value;
    
    internal override void InsertChild(DNode node)
    {
        throw new InvalidOperationException("DCharNode has no child nodes.");
    }

    public bool IsWhiteSpace() => char.IsWhiteSpace(Value);
}
