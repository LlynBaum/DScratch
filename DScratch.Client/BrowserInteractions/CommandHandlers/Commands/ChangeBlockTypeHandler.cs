using DScratch.Nodes;
using DScratch.Transactions;

namespace DScratch.Client.BrowserInteractions.CommandHandlers.Commands;

public static class ChangeBlockTypeHandler
{
    public static void Execute(ITransaction transaction, SelectionInfo selectionInfo, BlockType targetBlockType)
    {
        var selectedNodes = GetSelectedNodes(transaction, selectionInfo);
        if (!selectedNodes.Any())
        {
            throw new ArgumentException($"No nodes found for: {selectionInfo.AnchorId} - {selectionInfo.FocusId}");
        }

        var factory = GetFactory();
        foreach (var selectedNode in selectedNodes)
        {
            transaction.ReplaceNode(selectedNode, factory);
        }

        var (originOffset, _) = selectionInfo.GetConvertedOffsets();
        transaction.AddCursorPosition(selectedNodes.First().Id, originOffset);
        return;
        
        Func<DNode, DNode> GetFactory()
        {
            return targetBlockType switch
            {
                BlockType.Paragraph => transaction.NodeFactory.ParagraphFrom,
                BlockType.Heading1 => n => transaction.NodeFactory.HeadingFrom(n ,HeadingLevel.Level1),
                BlockType.Heading2 => n => transaction.NodeFactory.HeadingFrom(n ,HeadingLevel.Level2),
                BlockType.Heading3 => n => transaction.NodeFactory.HeadingFrom(n ,HeadingLevel.Level3),
                BlockType.Heading4 => n => transaction.NodeFactory.HeadingFrom(n ,HeadingLevel.Level4),
                BlockType.Heading5 => n => transaction.NodeFactory.HeadingFrom(n ,HeadingLevel.Level5),
                BlockType.Heading6 => n => transaction.NodeFactory.HeadingFrom(n ,HeadingLevel.Level6),
                _ => throw new ArgumentOutOfRangeException(nameof(targetBlockType))
            };
        }
    }

    private static IReadOnlyList<DNode> GetSelectedNodes(ITransaction transaction, SelectionInfo selectionInfo)
    {
        if (selectionInfo.Direction is SelectionDirection.None)
        {
            var node = transaction.FindNode(selectionInfo.AnchorNodeId)
                   ?? throw new ArgumentException($"Node with given id not found: {selectionInfo.AnchorId}");
            return [node];
        }
        
        var (originId, rightOriginId) = selectionInfo.GetConvertedNodeIds();
        
        var origin = transaction.FindNode(originId);
        if (origin is null)
        {
            throw new ArgumentException($"Node with given id not found: {selectionInfo.AnchorId}");
        }

        List<DNode> result = [];
        var current = origin;

        while (current is not null && current.Id != rightOriginId)
        {
            result.Add(current);
            current = current.RightOrigin;
        }

        if (current is not null) result.Add(current);
        return current is not null 
            ? result 
            : throw new NotImplementedException("Not even sure if this should be possible"); // When they don't share the same parent
    }
}