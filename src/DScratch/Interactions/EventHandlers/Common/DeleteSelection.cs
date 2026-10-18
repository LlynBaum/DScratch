using DScratch.Nodes;
using DScratch.Transactions;

namespace DScratch.Interactions.EventHandlers.Common;

internal static class DeleteSelection
{
    public static void Handle(KeyPressInfo keyPressInfo, ITransaction transaction, EventHandlerContext context)
    {
        var nodeSelection = SearchSelectedNodes(keyPressInfo, transaction);
        SetContextAnchors(context, nodeSelection);
        
        if (nodeSelection.IsInSameBlock())
        {
            transaction.DeleteRange(nodeSelection.DeleteStartId, nodeSelection.DeleteEndId);
        }
        else
        {
            DeleteAndMerge(nodeSelection, transaction);
        }
    }
    
    private static NodeSelection SearchSelectedNodes(KeyPressInfo keyPressInfo, ITransaction transaction)
    {
        var (anchorOffset, rightAnchorOffset) = keyPressInfo.Selection!.GetConvertedOffsets();
        var (anchorId, rightAnchorId) = keyPressInfo.Selection.GetConvertedNodeIds();
        var anchorNode = transaction.Document.Find(anchorId) ?? throw new InvalidOperationException($"Could not find a node for {anchorId}");
        var rightAnchorNode = transaction.Document.Find(rightAnchorId) ?? throw new InvalidOperationException($"Could not find a node for {rightAnchorId}");
        
        return new NodeSelection
        {
            DeleteStartNode = anchorNode,
            DeleteStartId = new NodeId(anchorId.Client, anchorId.Clock + anchorOffset),
            DeleteEndNode = rightAnchorNode,
            DeleteEndId = new NodeId(rightAnchorId.Client, rightAnchorId.Clock + rightAnchorOffset)
        };
    }

    private static void DeleteAndMerge(NodeSelection nodeSelection, ITransaction transaction)
    {
        var startBlock = nodeSelection.StartNodeParentBlock;
        transaction.DeleteRange(nodeSelection.DeleteStartId, startBlock.LastChild?.LastId);

        foreach (var node in nodeSelection.GetBlocksInBetween())
        {
            transaction.Delete(node.Id);
        }

        var endParent = nodeSelection.EndNodeParentBlock;
        transaction.DeleteRange(endParent.FirstChild?.Id, nodeSelection.DeleteEndId);
        transaction.MoveRange(nodeSelection.DeleteEndId, endParent.LastChild?.LastId, startBlock, startBlock.LastChild?.LastId);
        transaction.Delete(nodeSelection.DeleteEndNode.GetNearestBlock().Id);
    }
    
    private static void SetContextAnchors(EventHandlerContext context, NodeSelection nodeSelection)
    {
        if (nodeSelection.DeleteStartNode.Id != nodeSelection.DeleteStartId)
        {
            var contextAnchorId = new NodeId(nodeSelection.DeleteStartId.Client, nodeSelection.DeleteStartId.Clock - 1);
            context.SetAnchors(nodeSelection.DeleteStartNode, contextAnchorId);
        }
        else
        {
            var previousNode = nodeSelection.DeleteStartNode.PreviousSibling();
            if (previousNode is not null)
            {
                context.SetAnchors(previousNode, previousNode.LastId);
            }
            else
            {
                context.SetRightAnchor(nodeSelection.DeleteStartNode);
            }
        }
    }
    
    private class NodeSelection
    {
        public required NodeId DeleteStartId { get; init; }
        
        public required DNode DeleteStartNode { get; init; }
        
        public required NodeId DeleteEndId { get; init; }
        
        public required DNode DeleteEndNode { get; init; }

        public DNode StartNodeParentBlock
        {
            get
            {
                field ??= DeleteStartNode.GetNearestBlock();
                return field;
            }
        }
        
        public DNode EndNodeParentBlock
        {
            get
            {
                field ??= DeleteEndNode.GetNearestBlock();
                return field;
            }
        }

        public bool IsInSameBlock()
        {
            return StartNodeParentBlock == EndNodeParentBlock;
        }

        public IEnumerable<DNode> GetBlocksInBetween()
        {
            var current = StartNodeParentBlock.NextSibling();
            var endParent = EndNodeParentBlock;
            while (current is not null && current != endParent)
            {
                yield return current;
                current = current.NextSibling();
            }
        }
    }
}