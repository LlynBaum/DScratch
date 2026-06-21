using DScratch.Interactions.EventHandlers.Models;
using DScratch.Nodes;

namespace DScratch.Interactions;

public static class SelectionHelper
{
    public static DNodeInfo NearestTextNode(DNode target)
    {
        while (target is not null)
        {
            if (target is TextNode textNode) return new DNodeInfo(target, textNode.Length);
            target = target.LastChild;
        }
        
        return new DNodeInfo(target, 0);
    }
}