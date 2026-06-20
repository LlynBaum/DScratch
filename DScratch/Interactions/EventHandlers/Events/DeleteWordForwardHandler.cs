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
        
        var result = DNodeInfo.From(walker.Node, 0);
        
        var remainingCharacterOffset = DeleteWord(transaction, walker);
        if (walker.Node is not null)
        {
            transaction.SplitText(walker.Node, remainingCharacterOffset);
            transaction.Delete(walker.Node);
        }
        
        return result;
    }

    private static int DeleteWord(ITransaction transaction, TreeWalker<TextNode> walker)
    {
        var characterOffset = 0;
        while (walker.Node is not null && characterOffset < walker.Node.Length && char.IsWhiteSpace(walker.Node.TextContent[characterOffset]))
        {
            if (characterOffset == walker.Node.Length - 1)
            {
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
                transaction.Delete(walker.Node);
                walker.NextNode();
                characterOffset = 0;
            }
            characterOffset++;
        }

        return characterOffset;
    }
}