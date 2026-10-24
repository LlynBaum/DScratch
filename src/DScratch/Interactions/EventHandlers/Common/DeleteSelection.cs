using DScratch.Nodes;
using DScratch.Nodes.NodeTypes;
using DScratch.Transactions;

namespace DScratch.Interactions.EventHandlers.Common;

internal static class DeleteSelection
{
    public static void Handle(KeyPressInfo keyPressInfo, ITransaction transaction, EventHandlerContext context)
    {
        var nodeSelection = SearchSelectedNodes(keyPressInfo, transaction);
        
        if (nodeSelection.IsInSameBlock())
        {
            transaction.DeleteRange(nodeSelection.DeleteStartId, nodeSelection.DeleteEndId);
        }
        else
        {
            DeleteAndMerge(nodeSelection, transaction);
        }
        
        SetContextAnchors(context, nodeSelection);
    }
    
    private static NodeSelection SearchSelectedNodes(KeyPressInfo keyPressInfo, ITransaction transaction)
    {
        var (anchorOffset, rightAnchorOffset) = keyPressInfo.Selection!.GetConvertedOffsets();
        var (anchorId, rightAnchorId) = keyPressInfo.Selection.GetConvertedNodeIds();
        var anchorNode = transaction.Document.FindNode(anchorId);
        var rightAnchorNode = transaction.Document.FindNode(rightAnchorId);

        var anchor = GetAnchor(anchorNode, anchorOffset);
        var rightAnchor = GetRightAnchor(rightAnchorNode, rightAnchorOffset);
        
        return new NodeSelection
        {
            DeleteStartNode = anchor.node,
            DeleteStartId = anchor.nodeId,
            DeleteEndNode = rightAnchor.node,
            DeleteEndId = rightAnchor.nodeId
        };

        static (DNode node, NodeId nodeId) GetAnchor(DNode node, int offset)
        {
            return (node, new NodeId(node.Id.Client, node.Id.Clock + offset));
        }
        
        static (DNode node, NodeId nodeId) GetRightAnchor(DNode node, int offset)
        {
            var targetId = new NodeId(node.Id.Client, node.Id.Clock + offset);
            if (targetId.Clock <= node.LastId.Clock)
            {
                return (node, targetId);
            }

            return node.NextSibling() is { } nextSibling
                ? (nextSibling, nextSibling.Id)
                : (node.Parent!, node.Parent!.Id);
        }
    }

    private static void DeleteAndMerge(NodeSelection nodeSelection, ITransaction transaction)
    {
        var startBlock = nodeSelection.StartNodeParentBlock;
        transaction.DeleteRange(nodeSelection.DeleteStartId, null);

        foreach (var node in nodeSelection.GetBlocksInBetween())
        {
            transaction.Delete(node.Id);
        }

        var endParent = nodeSelection.EndNodeParentBlock;
        transaction.DeleteRange(endParent.FirstChild?.Id, nodeSelection.DeleteEndId);
        transaction.MoveRange(nodeSelection.DeleteEndId, null, startBlock, startBlock.LastChild?.LastId);
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
                field ??= DeleteStartNode is IBlockElement ? DeleteStartNode : DeleteStartNode.GetNearestBlock();
                return field;
            }
        }
        
        public DNode EndNodeParentBlock
        {
            get
            {
                field ??= DeleteEndNode is IBlockElement ? DeleteEndNode : DeleteEndNode.GetNearestBlock();
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