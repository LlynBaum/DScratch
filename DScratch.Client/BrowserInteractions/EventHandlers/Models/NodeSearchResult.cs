using DScratch.Nodes;

namespace DScratch.Client.BrowserInteractions.EventHandlers.Models;

public sealed record NodeSearchResult(NodeInfo Origin, NodeInfo RightOrigin);

public sealed record NodeInfo(DNode? Node, int AbsolutOffset, int RelativeOffset)
{
    public bool HasFoundNode => Node is not null;

    public int? AbsoluteOffsetIfPresent => HasFoundNode ? AbsolutOffset : null;
}