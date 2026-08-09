using System.Diagnostics.CodeAnalysis;
using DScratch.Nodes;

namespace DScratch.Interactions.EventHandlers.Models;

public readonly record struct DNodeInfo(DNode? Node, int Offset)
{
    [MemberNotNullWhen(true, nameof(Node))]
    [MemberNotNullWhen(true, nameof(AbsoluteOffsetIfPresent))]
    public bool HasFoundNode => Node is not null;

    public int? AbsoluteOffsetIfPresent => HasFoundNode ? Offset : null;
    
    public static DNodeInfo NotFound()
    {
        return new DNodeInfo(null, 0);
    }
    
    public static DNodeInfo From(DNode? node, int? offset)
    {
        if (node is not null && offset is null)
        {
            throw new ArgumentNullException(nameof(offset), "Offset must be not null, when node is not null");
        }

        return new DNodeInfo(node, offset ?? 0);
    }

}

public readonly record struct NodeInfo<TNode>(TNode? Node, int Offset) where TNode : DNode
{
    [MemberNotNullWhen(true, nameof(Node))]
    [MemberNotNullWhen(true, nameof(AbsoluteOffsetIfPresent))]
    public bool HasFoundNode => Node is not null;

    public int? AbsoluteOffsetIfPresent => HasFoundNode ? Offset : null;
    
    public static NodeInfo<TNode> NotFound()
    {
        return new NodeInfo<TNode>(null, 0);
    }
    
    public static NodeInfo<TNode> From(TNode? node, int? offset)
    {
        if (node is not null && offset is null)
        {
            throw new ArgumentNullException(nameof(offset), "Offset must be not null, when node is not null");
        }

        return new NodeInfo<TNode>(node, offset ?? 0);
    }
    
    public static implicit operator DNodeInfo(NodeInfo<TNode> nodeInfo)
    {
        return DNodeInfo.From(nodeInfo.Node, nodeInfo.AbsoluteOffsetIfPresent);
    }
}

