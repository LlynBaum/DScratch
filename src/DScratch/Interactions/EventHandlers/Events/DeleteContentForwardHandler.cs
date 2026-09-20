using DScratch.Interactions.EventHandlers.Common;
using DScratch.Interactions.EventHandlers.Models;
using DScratch.Nodes;
using DScratch.Transactions;

namespace DScratch.Interactions.EventHandlers.Events;

public class DeleteContentForwardHandler(IDScratchService dScratchService) : EventWithSelectionBase(dScratchService)
{
    public const string EventName = "deleteContentForward";
    
    protected override DNodeSearchResult HandleNoneSelection(KeyPressInfo keyPressInfo, ITransaction transaction,
        TextNode anchorTextNode)
    {
        var targetSelection = SimpleDeleteForward(keyPressInfo, transaction, anchorTextNode);
        if (targetSelection.HasFoundNode)
        {
            transaction.AddCursorPosition(targetSelection.Node.Id, targetSelection.Offset);
        }
        else if (anchorTextNode.GetNearestBlock() is { } parent && parent.NextSibling() is { } nextBlock)
        {
            transaction.AddCursorPosition(anchorTextNode.Id, anchorTextNode.Length); 
            transaction.MoveRange(nextBlock.FirstChild, null, parent, parent.LastChild);
            transaction.Delete(nextBlock);
        }

        return DNodeSearchResult.Empty;
    }

    protected override void HandleEmptyBlock(KeyPressInfo keyPressInfo, ITransaction transaction, DNode anchorNode)
    {
        var nextSibling = anchorNode.NextSibling();
        if (nextSibling is null) return;
        
        transaction.Delete(anchorNode);
        transaction.AddCursorPosition(nextSibling.Id, 0);
    }

    private static DNodeInfo SimpleDeleteForward(KeyPressInfo keyPressInfo, ITransaction transaction, TextNode targetTextNode)
    {
        var noteToDelete = transaction.SplitText(targetTextNode, keyPressInfo.Selection!.AnchorOffset);
        if (noteToDelete is null)
        {
            return DNodeInfo.NotFound();
        }
     
        transaction.SplitText(noteToDelete, 1);
        transaction.Delete(noteToDelete);
        return new DNodeInfo(targetTextNode, keyPressInfo.Selection.AnchorOffset);
    }
}