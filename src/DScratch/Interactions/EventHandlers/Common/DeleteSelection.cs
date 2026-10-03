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

    private static void DeleteAndMerge(NodeSelection nodeSelection, ITransaction transaction)
    {
        var startBlock = nodeSelection.AnchorParentBlock;
        transaction.DeleteRange(nodeSelection.AnchorNodeId, startBlock.LastChild?.LastId);

        foreach (var node in nodeSelection.GetBlocksInBetween())
        {
            transaction.Delete(node.Id);
        }

        var endParent = nodeSelection.RightAnchorParentBlock;
        transaction.DeleteRange(endParent.FirstChild?.Id, nodeSelection.RightAnchorNodeId);
        transaction.MoveRange(nodeSelection.RightAnchorNodeId, endParent.LastChild?.LastId, startBlock, startBlock.LastChild?.LastId);
    }
    
    private class NodeSelection
    {
        public required NodeId AnchorNodeId { get; init; }
        
        public required DNode AnchorNode { get; init; }
        
        public required NodeId RightAnchorNodeId { get; init; }
        
        public required DNode RightAnchorNode { get; init; }

        public DNode AnchorParentBlock
        {
            get
            {
                field ??= AnchorNode.GetNearestBlock();
                return field;
            }
        }
        
        public DNode RightAnchorParentBlock
        {
            get
            {
                field ??= RightAnchorNode.GetNearestBlock();
                return field;
            }
        }

        public bool IsInSameBlock()
        {
            return AnchorParentBlock == RightAnchorParentBlock;
        }

        public IEnumerable<DNode> GetBlocksInBetween()
        {
            var current = AnchorParentBlock.NextSibling();
            var endParent = RightAnchorParentBlock;
            while (current is not null && current != endParent)
            {
                yield return current;
                current = current.NextSibling();
            }
        }
    }
}