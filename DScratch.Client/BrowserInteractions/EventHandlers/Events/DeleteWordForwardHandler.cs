using DScratch.Client.BrowserInteractions.EventHandlers.Common;
using DScratch.Client.BrowserInteractions.EventHandlers.Models;
using DScratch.Nodes;
using DScratch.Nodes.NodeTypes;
using DScratch.Transactions;

namespace DScratch.Client.BrowserInteractions.EventHandlers.Events;

public class DeleteWordForwardHandler(IDScratchService dScratchService) : IEditorEventHandler
{
    public const string EventName = "deleteWordForward";
    
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
            
            if (!deletedNodeInfo.HasFoundNode && targetNode.GetNearestBlock() is { RightOrigin: not null } parent)
            {
                transaction.AddCursorPosition(targetTextNode.Id, targetTextNode.Length); 
                transaction.MoveRange(parent.RightOrigin.FirstChild, null, parent, parent.LastChild);
                transaction.Delete(parent.RightOrigin);
            }
            else if (deletedNodeInfo.HasFoundNode)
            {
                transaction.AddCursorPosition(deletedNodeInfo.Node.Id, deletedNodeInfo.Offset);
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