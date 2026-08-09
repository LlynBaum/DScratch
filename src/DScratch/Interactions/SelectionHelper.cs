using DScratch.Interactions.EventHandlers.Models;
using DScratch.Nodes;

namespace DScratch.Interactions;

public static class SelectionHelper
{
    public static NodeInfo<TextNode> NearestTextNode(DNode target)
    {
        var current = target;
        while (current is not null)
        {
            if (current is TextNode textNode) return new NodeInfo<TextNode>(textNode, textNode.Length);
            current = current.LastChild;
        }
        
        return NodeInfo<TextNode>.NotFound();
    }
}