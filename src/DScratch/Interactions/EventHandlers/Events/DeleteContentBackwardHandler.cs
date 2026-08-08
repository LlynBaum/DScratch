using DScratch.Interactions.EventHandlers.Common;
using DScratch.Interactions.EventHandlers.Models;
using DScratch.Nodes;
using DScratch.Transactions;

namespace DScratch.Interactions.EventHandlers.Events;

public class DeleteContentBackwardHandler(IDScratchService dScratchService) : EventWithSelectionBase(dScratchService)
{
    public const string EventName = "deleteContentBackward";

    protected override DNodeSearchResult HandleNoneSelection(KeyPressInfo keyPressInfo, ITransaction transaction,
        TextNode anchorTextNode)
    {
        var targetSelection = SimpleDeleteBackwards(keyPressInfo, transaction, anchorTextNode);
        if (targetSelection.HasFoundNode)
        {
            transaction.AddCursorPosition(targetSelection.Node!.Id, targetSelection.Offset);
        }
        else if (anchorTextNode.GetNearestBlock() is { Origin: not null } parent)
        {
            transaction.AddCursorPosition(anchorTextNode.Id, 0); 
            transaction.MoveRange(parent.FirstChild, null, parent.Origin, parent.Origin.LastChild);
            transaction.Delete(parent);
        }

        return DNodeSearchResult.Empty;
    }

    protected override void HandleEmptyBlock(KeyPressInfo keyPressInfo, ITransaction transaction, DNode anchorNode)
    {
        if (anchorNode.Origin is null) return;

        transaction.Delete(anchorNode);
        if (SelectionHelper.NearestTextNode(anchorNode.Origin) is { HasFoundNode: true } nodeInfo)
        {
            transaction.AddCursorPosition(nodeInfo.Node.Id, nodeInfo.Offset);
        }
        else
        {
            transaction.AddCursorPosition(anchorNode.Origin.Id, 0);
        }
    }

    private static DNodeInfo SimpleDeleteBackwards(KeyPressInfo keyPressInfo, ITransaction transaction, TextNode targetTextNode)
    {
        if (keyPressInfo.Selection!.AnchorOffset is 0)
        {
            return new DNodeInfo(null, 0);
        }

        transaction.SplitText(targetTextNode, keyPressInfo.Selection.AnchorOffset);
        var nodeToDelete = transaction.SplitText(targetTextNode, targetTextNode.Length - 1)!;
        transaction.Delete(nodeToDelete);

        return nodeToDelete.Origin is not null
            ? SelectionHelper.NearestTextNode(nodeToDelete.Origin)
            : new DNodeInfo(nodeToDelete.Parent, 0);
    }
}