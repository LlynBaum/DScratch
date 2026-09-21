using DScratch.Nodes.NodeTypes;

namespace DScratch.Nodes;

public class HeadingNode(HeadingLevel headingLevel, NodeId id, NodeId? origin, NodeId? rightOrigin, List<DNode>? childNodes = null) 
    : DNode(id, origin, rightOrigin, childNodes), IBlockElement
{
    public HeadingLevel HeadingLevel { get; } = headingLevel;
    
    public int GetTextLength() => DNodeHelper.GetTextLength(this);
    
    internal override void InsertChild(DNode node)
    {
        if (!node.IsTextOrInline())
        {
            throw new InvalidOperationException("Can only insert text or inline elements into block node.");
        }
        
        base.InsertChild(node);
    }
}