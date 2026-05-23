using DScratch.Nodes.NodeTypes;

namespace DScratch.Nodes;

public class TextNode(string id, DNode? origin, DNode? rightOrigin, List<DNode>? childNodes = null) 
    : DNode(id, origin, rightOrigin, childNodes), IShowText
{
    public int Length => ActiveChildNodes.Count();

    public string TextContent => ActiveChildNodes
        .Cast<CharNode>()
        .Aggregate(string.Empty, (text, node) => text + node.Value);

    internal override void InsertChild(DNode node)
    {
        if (node is not CharNode)
        {
            throw new InvalidOperationException("Can only insert DCharNode into TextNode.");
        }
        
        base.InsertChild(node);
    }
}