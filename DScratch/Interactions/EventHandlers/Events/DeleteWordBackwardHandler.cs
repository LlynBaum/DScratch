using DScratch.Interactions.EventHandlers.Common;
using DScratch.Interactions.EventHandlers.Models;
using DScratch.Nodes;
using DScratch.Transactions;

namespace DScratch.Interactions.EventHandlers.Events;

public class DeleteWordBackwardHandler(IDScratchService dScratchService) : EventWithSelectionBase(dScratchService)
{
    public const string EventName = "deleteWordBackward";
    
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

    protected override void HandleEmptyBlock(KeyPressInfo keyPressInfo, ITransaction transaction, DNode anchorNode)
    {
        transaction.Delete(anchorNode);
        var index = anchorNode.Parent?.IndexOf(anchorNode);
        var focusNode = index.HasValue ? anchorNode.Parent?.ChildAt(index.Value - 1) : null;
        
        if (focusNode is not null && GetCursorPosition(focusNode) is { HasFoundNode: true } nodeInfo)
        {
            transaction.AddCursorPosition(nodeInfo.Node.Id, nodeInfo.Offset);
        }
    }
    
    private static DNodeInfo GetCursorPosition(DNode target)
    {
        while (target is not null)
        {
            if (target is TextNode textNode) return new DNodeInfo(target, textNode.Length);
            target = target.LastChild;
        }
        
        return new DNodeInfo(target, 0);
    }

    private static DNodeInfo SimpleDeleteBackwards(KeyPressInfo keyPressInfo, ITransaction transaction, TextNode targetTextNode)
    {
        if (keyPressInfo.Selection.AnchorOffset is 0)
        {
            return DNodeInfo.NotFound();
        }

        var walker = TreeWalker<TextNode>.StartFrom(targetTextNode, targetTextNode.GetNearestBlock());
        if (walker.Node is null)
        {
            return DNodeInfo.NotFound();
        }
        
        transaction.SplitText(walker.Node, keyPressInfo.Selection.AnchorOffset);
        
        var previousNode = DeleteWord(transaction, walker, out var remainingCharacterOffset);
        var word = walker.Node is not null ? transaction.SplitText(walker.Node, remainingCharacterOffset + 1) : null;
        if (word is not null) transaction.Delete(word);

        return DNodeInfo.From(walker.Node ?? previousNode, walker.Node?.Length ?? 0);
    }

    private static TextNode? DeleteWord(ITransaction transaction, TreeWalker<TextNode> walker, out int index)
    {
        TextNode? previous = null;
        index = walker.Node!.Length - 1;
        
        while (index >= 0 && walker.Node is not null && char.IsWhiteSpace(walker.Node.TextContent[index]))
        {
            if (index == 0)
            {
                transaction.Delete(walker.Node);
                previous = walker.Node;
                walker.MovePrevious();
                index = walker.Node?.Length ?? 0;
            }
            index--;
        }

        while (index >= 0 && walker.Node is not null && !char.IsWhiteSpace(walker.Node.TextContent[index]))
        {
            if (index == 0)
            {
                transaction.Delete(walker.Node);
                previous = walker.Node;
                walker.MovePrevious();
                index = walker.Node?.Length ?? 0;
            }
            index--;
        }

        return previous;
    }
}