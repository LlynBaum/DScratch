using DScratch.Interactions.UserStates;
using DScratch.Marks;
using DScratch.Nodes;
using DScratch.Nodes.NodeTypes;
using DScratch.Transactions;

namespace DScratch.Interactions.CommandHandlers.Handlers;

public class UpdateMarkHandler(IDScratchService dScratchService, IUserStateService userStateService) : CommandBase<IMarkCommand>(dScratchService)
{
    protected override void Handle(ITransaction transaction, SelectionInfo selectionInfo, IMarkCommand command)
    {
        if (selectionInfo.Direction is SelectionDirection.None)
        {
            var anchorNode = transaction.Document.FindNode(selectionInfo.AnchorNodeId);
            if (anchorNode is IBlockElement)
            {
                command.Execute(transaction, anchorNode, [anchorNode]);
            }
            else
            {
                command.AddPending(userStateService);
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
        IMarkCommand command)
    {
        var selectedNodes = GetSelectedNodes(transaction, selectionInfo);
        var anchor = transaction.Document.FindNode(selectionInfo.AnchorNodeId)!;
        command.Execute(transaction, anchor, selectedNodes);

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
}
