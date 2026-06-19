using DScratch.Client.BrowserInteractions.EventHandlers.Common;
using DScratch.Client.BrowserInteractions.EventHandlers.Models;
using DScratch.Nodes;
using DScratch.Nodes.NodeTypes;
using DScratch.Transactions;

namespace DScratch.Client.BrowserInteractions.EventHandlers.Events;

public class DeleteContentBackwardHandler(IDScratchService dScratchService) : IEditorEventHandler
{
    public const string EventName = "deleteContentBackward";
    
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

            if (!deletedNodeInfo.HasFound && targetNode is IBlockTextNode && targetNode.OriginElement is IBlockTextNode blockTextNode)
            {
                transaction.AddCursorPosition(targetNode.OriginElement.Id, blockTextNode.GetTextLength());
                
                transaction.MoveRange(targetNode.FirstChild, null, targetNode.OriginElement, targetNode.OriginElement.LastChild);
                transaction.Delete(targetNode);
            }
            else if (deletedNodeInfo.HasFound)
            {
                transaction.AddCursorPosition(targetNode.Id, deletedNodeInfo.Offset);
            }
        }
        else
        {
            var nodeSearchResult = DeleteSelection.Handle(keyPressInfo, transaction);
            
            var cursorPosition = nodeSearchResult.Origin.AbsoluteOffsetIfPresent;
            var cursorTarget = nodeSearchResult.Origin.Node?.ParentElement ?? targetNode;
            if (cursorPosition is not null) transaction.AddCursorPosition(cursorTarget.Id, cursorPosition.Value);
        }
        
        return dScratchService.Apply(transaction);
    }

    private static NodeOffset SimpleDeleteBackwards(KeyPressInfo keyPressInfo, ITransaction transaction, TextNode targetTextNode)
    {
        if (keyPressInfo.Selection.AnchorOffset is 0)
        {
            return NodeOffset.NotFound();
        }
        
        transaction.SplitText(targetTextNode, keyPressInfo.Selection.AnchorOffset);
        var nodeToDelete = transaction.SplitText(targetTextNode, 1) ?? targetTextNode;
        transaction.Delete(nodeToDelete);
        
        return NodeOffset.From(nodeToDelete, keyPressInfo.Selection.AnchorOffset - 1);
    }
}