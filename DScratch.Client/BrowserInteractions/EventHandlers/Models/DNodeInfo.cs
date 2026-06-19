using System.Diagnostics.CodeAnalysis;
using DScratch.Nodes;

namespace DScratch.Client.BrowserInteractions.EventHandlers.Models;

public sealed record DNodeInfo(DNode? Node, int Offset) : NodeInfo<DNode>(Node, Offset);

public record NodeInfo<TNode>(TNode? Node, int Offset) where TNode : DNode
{
    [MemberNotNullWhen(true, nameof(Node))]
    [MemberNotNullWhen(true, nameof(AbsoluteOffsetIfPresent))]
    public bool HasFoundNode => Node is not null;

    public int? AbsoluteOffsetIfPresent => HasFoundNode ? Offset : null;
}

