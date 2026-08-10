using DScratch.Nodes.NodeTypes;

namespace DScratch.Nodes;

public class LinkNode(NodeId id, DNode? origin, DNode? rightOrigin, string href, string target, List<DNode>? childNodes = null) 
    : DNode(id, origin, rightOrigin, childNodes), IInlineElement
{
    public override string TagName => "a";

    public string Href { get; private set; } = href;

    public string Target { get; private set; } = target;
    
    internal override void InsertChild(DNode node)
    {
        if (!node.IsTextOrInline())
        {
            throw new InvalidOperationException("Can only insert text or inline elements into block node.");
        }
        
        base.InsertChild(node);
    }
}