using DScratch.Nodes;
using DScratch.Nodes.Marks;
using DScratch.Transactions;

namespace DScratch.Interactions.CommandHandlers.Commands;

public static class UpdateMarkHandler
{
    public static void Execute(
        ITransaction transaction, 
        SelectionInfo selectionInfo, 
        Mark mark,
        UpdateMarkAction action)
    {
        var selectedNodes = GetSelectedNodes(transaction, selectionInfo);

        switch (action)
        {
            case UpdateMarkAction.Toggle:
                var anchor = (TextNode)transaction.FindNode(selectionInfo.AnchorNodeId)!;
                var hasMark = anchor.Marks.Contains(mark);
                foreach (var selectedNode in selectedNodes)
                {
                    if (hasMark)
                    {
                        transaction.RemoveMark(selectedNode, mark.Key);
                    }
                    else
                    {
                        transaction.AddMark(selectedNode, mark);
                    }
                }
                break;
            case UpdateMarkAction.Add:
                foreach (var selectedNode in selectedNodes)
                {
                    transaction.AddMark(selectedNode, mark);
                }
                break;
            case UpdateMarkAction.Remove:
                foreach (var selectedNode in selectedNodes)
                {
                    transaction.RemoveMark(selectedNode, mark.Key);
                }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(action), action, null);
        }

        if (selectedNodes.Any())
        {
            var isBackward = selectionInfo.Direction == SelectionDirection.Backward;
            transaction.AddCursorPosition(new SelectionInfo
            {
                Direction = selectionInfo.Direction,
                AnchorId = isBackward ? selectedNodes.Last().Id.Value : selectedNodes.First().Id.Value,
                AnchorOffset = isBackward ? selectedNodes.Last().TextContent.Length : 0,
                FocusId = isBackward ? selectedNodes.First().Id.Value : selectedNodes.Last().Id.Value,
                FocusOffset = isBackward ? 0 : selectedNodes.Last().TextContent.Length,
            });
        }
    }

    private static IReadOnlyList<TextNode> GetSelectedNodes(ITransaction transaction, SelectionInfo selectionInfo)
    {
        if (selectionInfo.Direction is SelectionDirection.None)
        {
            // Currently there is no mark that would actually do anything with no selection.
            return [];
        }

        var (originId, rightOriginId) = selectionInfo.GetConvertedNodeIds();
        
        var origin = transaction.FindNode(originId);
        var rightOrigin = transaction.FindNode(rightOriginId);
        
        if (origin is null || rightOrigin is null)
        {
            throw new ArgumentException($"Node with given id not found: {selectionInfo.AnchorId} or {selectionInfo.FocusId}");
        }

        if (origin is not TextNode originText || rightOrigin is not TextNode)
        {
            throw new ArgumentException("Expected TextNode at selection.");
        }

        List<TextNode> result = [];
        var walker = TreeWalker<TextNode>.StartFrom(originText, transaction.Root);
        
        var (originOffset, rightOriginOffset) = selectionInfo.GetConvertedOffsets();
        if (originOffset > 0)
        {
            transaction.SplitText(originText, originOffset);
            walker.NextNode();
        }

        if (selectionInfo.AnchorId != selectionInfo.FocusId)
        {
            while (walker.Node is not null && walker.Node.Id != rightOrigin.Id)
            {
                result.Add(walker.Node);
                walker.NextNode();
            }
        }

        if (walker.Node is not null && rightOriginOffset > 0)
        {
            var offset = selectionInfo.AnchorId != selectionInfo.FocusId
                ? rightOriginOffset
                : rightOriginOffset - originOffset;
            transaction.SplitText(walker.Node, offset);
            result.Add(walker.Node);
        }
        return result;
    }
}

public enum UpdateMarkAction
{
    Toggle,
    Add,
    Remove
}