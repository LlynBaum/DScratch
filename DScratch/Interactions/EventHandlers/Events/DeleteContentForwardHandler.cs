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
        
        if (deletedNodeInfo.HasFoundNode)
        {
            transaction.AddCursorPosition(deletedNodeInfo.Node.Id, deletedNodeInfo.Offset);
        }   
        else if (!deletedNodeInfo.HasFoundNode && anchorTextNode.GetNearestBlock() is { RightOrigin: not null } parent)
        {
            transaction.AddCursorPosition(anchorTextNode.Id, anchorTextNode.Length); 
            transaction.MoveRange(parent.RightOrigin.FirstChild, null, parent, parent.LastChild);
            transaction.Delete(parent.RightOrigin);
        }

        return deletedNodeInfo;
    }

    protected override void HandleEmptyBlock(KeyPressInfo keyPressInfo, ITransaction transaction, DNode anchorNode)
    {
        transaction.Delete(anchorNode);
        var index = anchorNode.Parent?.IndexOf(anchorNode);
        var focusNode = index.HasValue ? anchorNode.Parent?.ChildAt(index.Value + 1) : null;
        if (focusNode is not null) transaction.AddCursorPosition(focusNode.Id, 0);
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