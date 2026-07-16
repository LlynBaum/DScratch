using DScratch.Interactions.EventHandlers.Models;
using DScratch.Nodes;

namespace DScratch.Interactions;

public static class SelectionHelper
{
    public static DNodeInfo NearestTextNode(DNode target)
    {
        var current = target;
        while (current is not null)
        {
            if (current is TextNode textNode) return new DNodeInfo(current, textNode.Length);
            current = current.LastChild;
        }
        
        return new DNodeInfo(current, 0);
    }
}