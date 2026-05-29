using DScratch.Nodes;

namespace DScratch.Client.BrowserInteractions.EventHandlers.Models;

public record NodeSearchResult(NodeInfo? Origin, NodeInfo? RightOrigin);

public record NodeInfo(DNode Node, int AbsolutOffset, int RelativeOffset)
{
    public static NodeInfo? TryCreate(DNode? node, int absolutOffset, int relativeOffset)
    {
        return node is not null ? new NodeInfo(node, absolutOffset, relativeOffset) : null;
    }
}