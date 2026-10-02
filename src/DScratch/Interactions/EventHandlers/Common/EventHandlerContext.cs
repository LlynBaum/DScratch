using System.Diagnostics.CodeAnalysis;
using DScratch.Interactions.EventHandlers.Models;
using DScratch.Nodes;

namespace DScratch.Interactions.EventHandlers.Common;

internal class EventHandlerContext
{
    public NodeId? AnchorNodeId { get; private set; }
    
    public DNode? AnchorNode { get; private set; }
    
    public NodeId? RightAnchorNodeId { get; private set; }
    
    public DNode? RightAnchorNode { get; private set; }

    [MemberNotNullWhen(true, nameof(AnchorNode), nameof(AnchorNodeId))]
    public bool HasAnchor => AnchorNode is not null;

    [MemberNotNullWhen(true, nameof(RightAnchorNode), nameof(RightAnchorNodeId))]
    public bool HasRightAnchor => RightAnchorNode is not null;

    private DNode? parentNode;

    public DNode GetParent()
    {
        return parentNode
               ?? AnchorNode?.Parent
               ?? RightAnchorNode?.Parent
               ?? throw new InvalidOperationException("Expected to have a parent node.");
    }

    public void SetCustomParent(DNode parent)
    {
        parentNode = parent;
    }

    public void SetAnchors(DNode anchor, NodeId anchorId)
    {
        AnchorNodeId = anchorId;
        AnchorNode = anchor;

        if (AnchorNode.LastId == anchorId)
        {
            RightAnchorNode = anchor.NextSibling();
            RightAnchorNodeId = RightAnchorNode?.Id;
        }
        else
        {
            RightAnchorNode = anchor;
            RightAnchorNodeId = new NodeId(AnchorNodeId.Value.Client, AnchorNodeId.Value.Clock + 1);
        }
    }

    public void SetRightAnchor(DNode? rightAnchor)
    {
        AnchorNodeId = null;
        AnchorNode = null;
        RightAnchorNodeId = rightAnchor?.Id;
        RightAnchorNode = rightAnchor;
    }
}