using DScratch.Nodes.NodeTypes;

namespace DScratch.Nodes;

public class TextNode(string id, DNode? origin, DNode? rightOrigin, DNode? parent, List<DNode>? childNodes = null) 
    : DNode(id, origin, rightOrigin, parent, childNodes), IText
{
    public int Length => ChildNodes.Count;

    public string TextContent => ChildNodes.Cast<CharNode>().Aggregate(string.Empty, (text, node) => text + node.Value);

    internal override void InsertChild(DNode node)
    {
        if (node is not CharNode)
        {
            throw new InvalidOperationException("Can only insert DCharNode into TextNode.");
        }
        
        base.InsertChild(node);
    }
}