using DScratch.Interactions.EventHandlers.Common;
using DScratch.Interactions.EventHandlers.Models;
using DScratch.Nodes;
using DScratch.Transactions;

namespace DScratch.Interactions.EventHandlers.Events;

public class DeleteContentForwardHandler(IDScratchService dScratchService) : EventWithSelectionBase(dScratchService)
{
    public const string EventName = "deleteContentForward";
    
    protected override DNodeInfo HandleNoneSelection(KeyPressInfo keyPressInfo, ITransaction transaction, TextNode anchorTextNode)
    {
        var deletedNodeInfo = SimpleDeleteForward(keyPressInfo, transaction, anchorTextNode);
            
        if (!deletedNodeInfo.HasFoundNode && anchorTextNode.GetNearestBlock() is { RightOrigin: not null } parent)
        {
            transaction.AddCursorPosition(anchorTextNode.Id, anchorTextNode.Length); 
            transaction.MoveRange(parent.RightOrigin.FirstChild, null, parent, parent.LastChild);
            transaction.Delete(parent.RightOrigin);
        }
        else if (deletedNodeInfo.HasFoundNode)
        {
            transaction.AddCursorPosition(deletedNodeInfo.Node.Id, deletedNodeInfo.Offset);
        }

        return deletedNodeInfo;
    }
    
    private static DNodeInfo SimpleDeleteForward(KeyPressInfo keyPressInfo, ITransaction transaction, TextNode targetTextNode)
    {
        var noteToDelete = transaction.SplitText(targetTextNode, keyPressInfo.Selection.AnchorOffset);
        if (noteToDelete is not null)
        {
            transaction.SplitText(noteToDelete, 1);
            transaction.Delete(noteToDelete);
        }

        return new DNodeInfo(noteToDelete, keyPressInfo.Selection.AnchorOffset);
    }
}