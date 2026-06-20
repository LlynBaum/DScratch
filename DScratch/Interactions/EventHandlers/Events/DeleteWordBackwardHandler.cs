using DScratch.Interactions.EventHandlers.Common;
using DScratch.Interactions.EventHandlers.Models;
using DScratch.Nodes;
using DScratch.Transactions;

namespace DScratch.Interactions.EventHandlers.Events;

public class DeleteWordBackwardHandler(IDScratchService dScratchService) : IEditorEventHandler
{
    public const string EventName = "deleteWordBackward";
    
    public TransactionResult Handle(KeyPressInfo keyPressInfo)
    {
        var transaction = dScratchService.StartTransaction();

        var targetNode = transaction.FindNode(keyPressInfo.Selection.AnchorNodeId);
        if (targetNode is not TextNode targetTextNode)
        {
            throw new ArgumentException($"Expected TextNode at {keyPressInfo.Selection.AnchorId}");
        }

        if (keyPressInfo.Selection.Direction is SelectionDirection.None)
        {
            var deletedNodeInfo = SimpleDeleteBackwards(keyPressInfo, transaction, targetTextNode);
            
            if (!deletedNodeInfo.HasFoundNode && targetNode.GetNearestBlock() is { Origin: not null } parent)
            {
                transaction.AddCursorPosition(targetNode.Id, 0); 
                transaction.MoveRange(parent.FirstChild, null, parent.Origin, parent.Origin.LastChild);
                transaction.Delete(parent);
            }
            else if (deletedNodeInfo.HasFoundNode)
            {
                transaction.AddCursorPosition(deletedNodeInfo.Node.Origin?.Id ?? deletedNodeInfo.Node.Id, deletedNodeInfo.Offset);
            }
        }
        else
        {
            var nodeSearchResult = DeleteSelection.Handle(keyPressInfo, transaction);
            var cursorPosition = nodeSearchResult.Origin.AbsoluteOffsetIfPresent;
            var cursorTarget = nodeSearchResult.Origin.Node ?? targetNode;
            if (cursorPosition is not null) transaction.AddCursorPosition(cursorTarget.Id, cursorPosition.Value);
        }

        return dScratchService.Apply(transaction);
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