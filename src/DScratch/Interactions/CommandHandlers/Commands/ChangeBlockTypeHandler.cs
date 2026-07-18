using DScratch.Nodes;
using DScratch.Transactions;

namespace DScratch.Interactions.CommandHandlers.Commands;

public static class ChangeBlockTypeHandler
{
    public static void Execute(ITransaction transaction, SelectionInfo selectionInfo, BlockNodeType targetBlockNodeType)
    {
        var selectedNodes = GetSelectedNodes(transaction, selectionInfo);
        if (!selectedNodes.Any())
        {
            throw new ArgumentException($"No nodes found for anchor {selectionInfo.AnchorId} and focus {selectionInfo.FocusId}");
        }

        var factory = GetFactory();
        foreach (var selectedNode in selectedNodes)
        {
            transaction.ReplaceNode(selectedNode, factory);
        }

        transaction.AddCursorPosition(selectionInfo);
        return;
        
        Func<DNode, DNode> GetFactory()
        {
            return targetBlockNodeType switch
            {
                BlockNodeType.Paragraph => transaction.NodeFactory.ParagraphFrom,
                BlockNodeType.Heading1 => n => transaction.NodeFactory.HeadingFrom(n ,HeadingLevel.Level1),
                BlockNodeType.Heading2 => n => transaction.NodeFactory.HeadingFrom(n ,HeadingLevel.Level2),
                BlockNodeType.Heading3 => n => transaction.NodeFactory.HeadingFrom(n ,HeadingLevel.Level3),
                BlockNodeType.Heading4 => n => transaction.NodeFactory.HeadingFrom(n ,HeadingLevel.Level4),
                BlockNodeType.Heading5 => n => transaction.NodeFactory.HeadingFrom(n ,HeadingLevel.Level5),
                BlockNodeType.Heading6 => n => transaction.NodeFactory.HeadingFrom(n ,HeadingLevel.Level6),
                _ => throw new ArgumentOutOfRangeException(nameof(targetBlockNodeType))
            };
        }
    }

    private static IReadOnlyList<DNode> GetSelectedNodes(ITransaction transaction, SelectionInfo selectionInfo)
    {
        if (selectionInfo.Direction is SelectionDirection.None)
        {
            var node = transaction.Document.FindNode(selectionInfo.AnchorNodeId)?.GetNearestBlock()
                   ?? throw new ArgumentException($"Node with given id not found: {selectionInfo.AnchorId}");
            return [node];
        }
        
        var (originId, rightOriginId) = selectionInfo.GetConvertedNodeIds();
        
        var origin = transaction.Document.FindNode(originId)?.GetNearestBlock();
        var rightOrigin = transaction.Document.FindNode(rightOriginId)?.GetNearestBlock();
        
        if (origin is null || rightOrigin is null)
        {
            throw new ArgumentException($"Node with given id not found: {selectionInfo.AnchorId} or {selectionInfo.FocusId}");
        }

        if (origin.Parent?.Id != rightOrigin.Parent?.Id)
        {
            throw new ArgumentException($"Nodes {selectionInfo.AnchorId} and {selectionInfo.FocusId} do not share the same parent.");
        }

        List<DNode> result = [];
        var current = origin;

        while (current is not null && current.Id != rightOrigin.Id)
        {
            result.Add(current);
            current = current.RightOrigin;
        }

        if (current is not null) result.Add(current);
        return current is not null 
            ? result 
            : throw new ArgumentException($"Nodes {selectionInfo.AnchorId} and {selectionInfo.FocusId} do not share the same parent.");
    }
}