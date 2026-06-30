using DScratch.Interactions.EventHandlers.Common;
using DScratch.Interactions.EventHandlers.Models;
using DScratch.Nodes;
using DScratch.Transactions;

namespace DScratch.Interactions.EventHandlers.Events;

public class DeleteWordBackwardHandler(IDScratchService dScratchService) : EventWithSelectionBase(dScratchService)
{
    public const string EventName = "deleteWordBackward";
    
    protected override DNodeSearchResult HandleNoneSelection(KeyPressInfo keyPressInfo, ITransaction transaction,
        TextNode anchorTextNode)
    {
        var targetSelection = SimpleDeleteBackwards(keyPressInfo, transaction, anchorTextNode);
        if (targetSelection.HasFoundNode)
        {
            transaction.AddCursorPosition(targetSelection.Node.Origin?.Id ?? targetSelection.Node.Id, targetSelection.Offset);
        }
        else if (!targetSelection.HasFoundNode && anchorTextNode.GetNearestBlock() is { Origin: not null } parent)
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
        if (keyPressInfo.Selection.AnchorOffset is 0)
        {
            return DNodeInfo.NotFound();
        }

        var parent = targetTextNode.GetNearestBlock();
        var walker = TreeWalker<TextNode>.StartFrom(targetTextNode, parent);
        if (walker.Node is null)
        {
            return DNodeInfo.NotFound();
        }
        
        transaction.SplitText(walker.Node, keyPressInfo.Selection.AnchorOffset);
        
        return DeleteWord(transaction, walker);
    }

    private static DNodeInfo DeleteWord(ITransaction transaction, TreeWalker<TextNode> walker)
    {
        var hasFoundNoe = false;
        var index = walker.Node!.Length - 1;
        
        while (index >= 0 && walker.Node is not null && char.IsWhiteSpace(walker.Node.TextContent[index]))
        {
            if (index == 0)
            {
                hasFoundNoe = true;
                transaction.Delete(walker.Node);
                walker.MovePrevious();
                index = walker.Node?.Length ?? 0;
            }
            index--;
        }

        while (index >= 0 && walker.Node is not null && !char.IsWhiteSpace(walker.Node.TextContent[index]))
        {
            if (index == 0)
            {
                hasFoundNoe = true;
                transaction.Delete(walker.Node);
                walker.MovePrevious();
                index = walker.Node?.Length ?? 0;
            }
            index--;
        }
        
        var word = walker.Node is not null ? transaction.SplitText(walker.Node, index + 1) : null;
        if (!hasFoundNoe && word is null)
        {
            return DNodeInfo.NotFound();
        }

        if (word is not null) transaction.Delete(word);
        return word?.Origin is not null
            ? SelectionHelper.NearestTextNode(word.Origin)
            : new DNodeInfo(word?.Parent ?? walker.Parent, 0);
    }
}