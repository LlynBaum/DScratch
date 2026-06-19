using DScratch.Client.BrowserInteractions.EventHandlers.Common;
using DScratch.Client.BrowserInteractions.EventHandlers.Models;
using DScratch.Nodes;
using DScratch.Nodes.NodeTypes;
using DScratch.Transactions;

namespace DScratch.Client.BrowserInteractions.EventHandlers.Events;

public class DeleteContentForwardHandler(IDScratchService dScratchService) : IEditorEventHandler
{
    public const string EventName = "deleteContentForward";
    
    public TransactionResult Handle(KeyPressInfo keyPressInfo)
    {
        var transaction = dScratchService.StartTransaction();

        var targetNode = transaction.FindNode(keyPressInfo.Selection.AnchorNodeId);
        if (targetNode is not TextNode targetTextNode)
        {
            throw new ArgumentException($"Parent with given path not found: {keyPressInfo.Selection.AnchorId}");
        }
        
        if (keyPressInfo.Selection.Direction is SelectionDirection.None)
        {
            var deletedNodeInfo = SimpleDeleteForward(keyPressInfo, transaction, targetTextNode);
            
            if (!deletedNodeInfo.HasFound && targetNode is IBlockTextNode blockTextNode && targetNode.RightOriginElement is IBlockTextNode)
            {
                transaction.AddCursorPosition(targetNode.RightOriginElement.Id, blockTextNode.GetTextLength());
                
                transaction.MoveRange(targetNode.FirstChild, null, targetNode.RightOriginElement, null);
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
    
    private static NodeOffset SimpleDeleteForward(KeyPressInfo keyPressInfo, ITransaction transaction, TextNode targetTextNode)
    {
        var noteToDelete = transaction.SplitText(targetTextNode, keyPressInfo.Selection.AnchorOffset);
        if (noteToDelete is not null)
        {
            transaction.SplitText(noteToDelete, 1);
            transaction.Delete(noteToDelete);
        }

        return NodeOffset.From(noteToDelete, keyPressInfo.Selection.AnchorOffset);
    }
}