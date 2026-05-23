using DScratch.Nodes.NodeTypes;

namespace DScratch.Nodes;

public class ParagraphNode(string id, DNode? origin, DNode? rightOrigin, List<DNode>? childNodes = null)
    : DNode(id, origin, rightOrigin, childNodes), IElement
{
    public string TagName => "p";

    internal override void InsertChild(DNode node)
    {
        if (!(node.IsTextOrInline() || node.IsInlineNode()))
        {
            throw new InvalidOperationException("Can only insert text or inline elements into block node.");
        }
        
        base.InsertChild(node);
    }
}