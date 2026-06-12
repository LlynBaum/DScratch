using DScratch.Nodes;

namespace DScratch.Client.BrowserInteractions.EventHandlers.Models;

public sealed record DNodeInfo(DNode? Node, int AbsolutOffset) : NodeInfo<DNode>(Node, AbsolutOffset);

public record NodeInfo<TNode>(TNode? Node, int AbsolutOffset) where TNode : DNode
{
    public bool HasFoundNode => Node is not null;

    public int? AbsoluteOffsetIfPresent => HasFoundNode ? AbsolutOffset : null;
}

