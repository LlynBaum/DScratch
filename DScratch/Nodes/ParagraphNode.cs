using DScratch.Nodes.NodeTypes;

namespace DScratch.Nodes;

public class ParagraphNode(NodeId id, DNode? origin, DNode? rightOrigin, List<DNode>? childNodes = null)
    : DNode(id, origin, rightOrigin, childNodes), IBlockTextNode
{
    public override string TagName => "p";
    
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