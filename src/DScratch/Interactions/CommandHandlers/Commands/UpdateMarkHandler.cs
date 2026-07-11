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
                foreach (var selectedNode in selectedNodes)
                {
                    if (selectedNode.Marks.Contains(mark)) // TODO actually wrong, check the Anchor Node for the mark, the rest wil be added/removed based on the anchor
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
        if (transaction.SplitText(originText, originOffset) is null)
        {
            walker.NextNode();
        }

        while (walker.Node is not null && walker.Node.Id != rightOrigin.Id)
        {
            result.Add(walker.Node);
            walker.NextNode();
        }

        if (walker.Node is not null && rightOriginOffset > 0)
        {
            transaction.SplitText(walker.Node, rightOriginOffset);
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