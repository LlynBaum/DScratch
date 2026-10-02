using DScratch.Interactions.EventHandlers.Models;
using DScratch.Nodes;
using DScratch.Transactions;

namespace DScratch.Interactions.EventHandlers.Common;

internal static class DeleteSelection
{
    public static void Handle(KeyPressInfo keyPressInfo, ITransaction transaction, EventHandlerContext context)
    {
        var nodeSelection = SearchSelectedNodes(keyPressInfo, transaction);
        context.SetAnchors(nodeSelection.AnchorNode, nodeSelection.AnchorNodeId);
        if (nodeSelection.IsInSameBlock())
        {
            transaction.DeleteRange(nodeSelection.AnchorNodeId, nodeSelection.RightAnchorNodeId); // TODO: delete range working with ids
        }
        else
        {
            DeleteAndMerge(nodeSelection, transaction); // TODO: Delete range at start and end, delete everything in between, move tail start para to merge them
        }
    }
    
    private static NodeSelection SearchSelectedNodes(KeyPressInfo keyPressInfo, ITransaction transaction)
    {
        var (anchorOffset, rightAnchorOffset) = keyPressInfo.Selection!.GetConvertedOffsets();
        var (anchorId, rightAnchorId) = keyPressInfo.Selection.GetConvertedNodeIds();
        var anchorNode = transaction.Document.FindNode(anchorId) ?? throw new InvalidOperationException($"Could not find a node for {anchorId}");
        var rightAnchorNode = transaction.Document.FindNode(rightAnchorId) ?? throw new InvalidOperationException($"Could not find a node for {rightAnchorId}");
        
        return new NodeSelection
        {
            AnchorNode = anchorNode,
            AnchorNodeId = new NodeId(anchorId.Client, anchorId.Clock + anchorOffset),
            RightAnchorNode = rightAnchorNode,
            RightAnchorNodeId = new NodeId(rightAnchorId.Client, rightAnchorId.Clock + rightAnchorOffset)
        };
    }

    private static void DeleteAndMerge(NodeSearchResult<TextNode> nodeSearchResult, ITransaction transaction)
    {
        var deleteStart = nodeSearchResult.Origin.HasFoundNode 
            ? transaction.SplitText(nodeSearchResult.Origin.Node, nodeSearchResult.Origin.Offset) 
            : null;
        
        if(nodeSearchResult.RightOrigin.HasFoundNode) transaction.SplitText(nodeSearchResult.RightOrigin.Node, nodeSearchResult.RightOrigin.Offset);

        transaction.DeleteRange(deleteStart, null);
        transaction.DeleteRange(null, nodeSearchResult.RightOrigin.Node);
        
        transaction.MoveRange(nodeSearchResult.RightOrigin.Node?.NextSibling(), null, deleteStart?.Parent!, deleteStart?.Parent?.LastChild);
        transaction.DeleteRange(deleteStart?.Parent?.NextSibling(), nodeSearchResult.RightOrigin.Node?.Parent);
    }
    
    private class NodeSelection
    {
        public required NodeId AnchorNodeId { get; init; }
        
        public required DNode AnchorNode { get; init; }
        
        public required NodeId RightAnchorNodeId { get; init; }
        
        public required DNode RightAnchorNode { get; init; }

        public bool IsInSameBlock()
        {
            return AnchorNode.GetNearestBlock() == RightAnchorNode.GetNearestBlock();
        }

        public IEnumerable<DNode> GetBlocksInBetween()
        {
            var current = AnchorNode.NextSibling();
            while (current is not null && current != RightAnchorNode)
            {
                yield return current;
                current = current.NextSibling();
            }
        }
    }
}