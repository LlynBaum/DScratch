using DScratch.Interactions.EventHandlers.Common;
using DScratch.Interactions.EventHandlers.Models;
using DScratch.Nodes;
using DScratch.Transactions;

namespace DScratch.Interactions.EventHandlers.Events;

public class DeleteContentBackwardHandler(IDScratchService dScratchService) : EventWithSelectionBase(dScratchService)
{
    public const string EventName = "deleteContentBackward";

    protected override DNodeInfo HandleNoneSelection(KeyPressInfo keyPressInfo, ITransaction transaction, TextNode anchorTextNode)
    {
        var deletedNodeInfo = SimpleDeleteBackwards(keyPressInfo, transaction, anchorTextNode);

        if (deletedNodeInfo.HasFoundNode)
        {
            transaction.AddCursorPosition(deletedNodeInfo.Node.Origin?.Id ?? deletedNodeInfo.Node.Id, deletedNodeInfo.Offset);
        }
        else if (!deletedNodeInfo.HasFoundNode && anchorTextNode.GetNearestBlock() is { Origin: not null } parent)
        {
            transaction.AddCursorPosition(anchorTextNode.Id, 0); 
            transaction.MoveRange(parent.FirstChild, null, parent.Origin, parent.Origin.LastChild);
            transaction.Delete(parent);
        }

        return deletedNodeInfo;
    }

    private static DNodeInfo SimpleDeleteBackwards(KeyPressInfo keyPressInfo, ITransaction transaction, TextNode targetTextNode)
    {
        if (keyPressInfo.Selection.AnchorOffset is 0)
        {
            return new DNodeInfo(null, 0);
        }
        
        transaction.SplitText(targetTextNode, keyPressInfo.Selection.AnchorOffset);
        var nodeToDelete = transaction.SplitText(targetTextNode, 1) ?? targetTextNode;
        transaction.Delete(nodeToDelete);
        
        return new DNodeInfo(nodeToDelete, keyPressInfo.Selection.AnchorOffset - 1);
    }
}