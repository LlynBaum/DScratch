using DScratch.Interactions.EventHandlers.Common;
using DScratch.Interactions.EventHandlers.Models;
using DScratch.Nodes;
using DScratch.Transactions;

namespace DScratch.Interactions.EventHandlers.Events;

public class DeleteWordForwardHandler(IDScratchService dScratchService) : EventWithSelectionBase(dScratchService)
{
    public const string EventName = "deleteWordForward";
    
    protected override DNodeInfo HandleNoneSelection(KeyPressInfo keyPressInfo, ITransaction transaction, TextNode anchorTextNode)
    {
        var deletedNodeInfo = SimpleDeleteBackwards(keyPressInfo, transaction, anchorTextNode);
            
        if (deletedNodeInfo.HasFoundNode)
        {
            transaction.AddCursorPosition(deletedNodeInfo.Node.Id, deletedNodeInfo.Offset);
        }
        else if (anchorTextNode.GetNearestBlock() is { RightOrigin: not null } parent)
        {
            transaction.AddCursorPosition(anchorTextNode.Id, anchorTextNode.Length); 
            transaction.MoveRange(parent.RightOrigin.FirstChild, null, parent, parent.LastChild);
            transaction.Delete(parent.RightOrigin);
        }

        return DNodeInfo.NotFound();
    }

    protected override void HandleEmptyBlock(KeyPressInfo keyPressInfo, ITransaction transaction, DNode anchorNode)
    {
        if (anchorNode.RightOrigin is null) return;
        
        transaction.Delete(anchorNode);
        var index = anchorNode.Parent?.IndexOf(anchorNode);
        var focusNode = index.HasValue ? anchorNode.Parent?.ChildAt(index.Value + 1) : null;
        if (focusNode is not null) transaction.AddCursorPosition(focusNode.Id, 0);
    }

    private static DNodeInfo SimpleDeleteBackwards(KeyPressInfo keyPressInfo, ITransaction transaction, TextNode targetTextNode)
    {
        var walker = TreeWalker<TextNode>.StartFrom(targetTextNode, targetTextNode.GetNearestBlock());
        if (walker.Node is null)
        {
            return DNodeInfo.NotFound();
        }
        
        var rightNode = transaction.SplitText(walker.Node, keyPressInfo.Selection.AnchorOffset);
        if (rightNode is null || keyPressInfo.Selection.AnchorOffset > 0)
        {
            walker.NextNode();
        }

        var beforeDelete = walker.Node?.Origin is not null 
            ? SelectionHelper.NearestTextNode(walker.Node.Origin) 
            : DNodeInfo.NotFound();

        var afterDelete = DeleteWord(transaction, walker);
        return beforeDelete.HasFoundNode ? beforeDelete : afterDelete;
    }

    private static DNodeInfo DeleteWord(ITransaction transaction, TreeWalker<TextNode> walker)
    {
        var hasFoundNoe = false;
        var characterOffset = 0;
        while (walker.Node is not null && characterOffset < walker.Node.Length && char.IsWhiteSpace(walker.Node.TextContent[characterOffset]))
        {
            if (characterOffset == walker.Node.Length - 1)
            {
                hasFoundNoe = true;
                transaction.Delete(walker.Node);
                walker.NextNode();
                characterOffset = 0;
            }
            characterOffset++;
        }

        while (walker.Node is not null && characterOffset < walker.Node.Length && !char.IsWhiteSpace(walker.Node.TextContent[characterOffset]))
        {
            if (characterOffset == walker.Node.Length - 1)
            {
                hasFoundNoe = true;
                transaction.Delete(walker.Node);
                walker.NextNode();
                characterOffset = 0;
            }
            characterOffset++;
        }
        
        var word = walker.Node is not null ? transaction.SplitText(walker.Node, characterOffset) : null;
        if (!hasFoundNoe && word is null)
        {
            return DNodeInfo.NotFound();
        }

        if (walker.Node is not null) transaction.Delete(walker.Node);
        return word is not null
            ? new DNodeInfo(word, 0)
            : new DNodeInfo(walker.Parent, 0);
    }
}