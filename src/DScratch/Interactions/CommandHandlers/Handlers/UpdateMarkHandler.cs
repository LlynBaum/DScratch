using DScratch.Interactions.CommandHandlers.Commands;
using DScratch.Interactions.UserStates;
using DScratch.Nodes;
using DScratch.Nodes.Marks;
using DScratch.Nodes.NodeTypes;
using DScratch.Transactions;

namespace DScratch.Interactions.CommandHandlers.Handlers;

public class UpdateMarkHandler(IDScratchService dScratchService, IUserStateService userStateService) : CommandBase<UpdateMarkCommand>(dScratchService)
{
    protected override void Handle(ITransaction transaction, SelectionInfo selectionInfo, UpdateMarkCommand command)
    {
        if (selectionInfo.Direction is SelectionDirection.None)
        {
            var anchorNode = transaction.Document.FindNode(selectionInfo.AnchorNodeId);
            if (anchorNode is IBlockElement)
            {
                UpdateEmptyBlockMarks(transaction, command, anchorNode);
            }
            else
            {
                UpdatePendingMarks(command.Key, command.Value, command.Action);
            }
        }
        else
        {
            HandleSelection(transaction, selectionInfo, command);
        }
    }

    private static void HandleSelection(
        ITransaction transaction,
        SelectionInfo selectionInfo,
        UpdateMarkCommand command)
    {
        var selectedNodes = GetSelectedNodes(transaction, selectionInfo);

        switch (command.Action)
        {
            case UpdateMarkAction.Toggle:
                var anchor = transaction.Document.FindNode(selectionInfo.AnchorNodeId)!;
                var hasMark = anchor.Marks.ContainsKey(command.Key);
                foreach (var selectedNode in selectedNodes)
                {
                    if (hasMark)
                    {
                        transaction.RemoveMark(selectedNode, command.Key);
                    }
                    else
                    {
                        transaction.AddMark(selectedNode, command.Key, command.Value!);
                    }
                }
                break;
            case UpdateMarkAction.Add:
                foreach (var selectedNode in selectedNodes)
                {
                    transaction.AddMark(selectedNode, command.Key, command.Value!);
                }
                break;
            case UpdateMarkAction.Remove:
                foreach (var selectedNode in selectedNodes)
                {
                    transaction.RemoveMark(selectedNode, command.Key);
                }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(command.Action), command.Action, null);
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
        
        var origin = transaction.Document.FindNode(originId);
        var rightOrigin = transaction.Document.FindNode(rightOriginId);
        
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
    
    private static void UpdateEmptyBlockMarks(
        ITransaction transaction,
        UpdateMarkCommand command, 
        DNode anchorNode)
    {
        switch (command.Action)
        {
            case UpdateMarkAction.Toggle:
                var hasMark = anchorNode.Marks.ContainsKey(command.Key);
                if (hasMark)
                {
                    transaction.RemoveMark(anchorNode, command.Key);
                }
                else
                {
                    transaction.AddMark(anchorNode, command.Key, command.Value!);
                }
                break;
            case UpdateMarkAction.Add:
                transaction.AddMark(anchorNode, command.Key, command.Value!);
                break;
            case UpdateMarkAction.Remove:
                transaction.RemoveMark(anchorNode, command.Key);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(command.Action), command.Action, null);
        }
    }
    
    private void UpdatePendingMarks(MarkKey key, string? value, UpdateMarkAction action)
    {
        switch (action)
        {
            case UpdateMarkAction.Remove:
                userStateService.RemovePendingMark(key);
                break;
            case UpdateMarkAction.Add:
                userStateService.AddPendingMark(key, value!);
                break;
            case UpdateMarkAction.Toggle:
                if (userStateService.CheckMark(key, out _))
                {
                    userStateService.RemovePendingMark(key);
                }
                else
                {
                    userStateService.AddPendingMark(key, value!);
                }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(action), action, null);
        }
    }
}

public enum UpdateMarkAction
{
    Toggle,
    Add,
    Remove
}