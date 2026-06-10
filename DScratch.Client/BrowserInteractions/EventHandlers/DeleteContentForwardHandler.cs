using DScratch.Client.BrowserInteractions.EventHandlers.Common;
using DScratch.Client.BrowserInteractions.EventHandlers.Models;
using DScratch.Nodes;
using DScratch.Transactions;

namespace DScratch.Client.BrowserInteractions.EventHandlers;

public class DeleteContentForwardHandler(IDScratchService dScratchService) : IEditorEventHandler
{
    public const string EventName = "deleteContentForward";
    
    public TransactionResult Handle(KeyPressInfo keyPressInfo)
    {
        var transaction = dScratchService.StartTransaction();
        
        var parent = transaction.FindNode(keyPressInfo.Selection.AnchorNodeId);
        if (parent is null)
        {
            throw new ArgumentException($"Parent with given path not found: {keyPressInfo.Selection.AnchorId}");
        }
        
        if (keyPressInfo.Selection.Direction is SelectionDirection.None)
        {
            var deletedNodeInfo = SimpleDeleteForward(keyPressInfo, transaction, parent);
            
            // TODO: for some reason, it deletes the first char in the next para first
            if (!deletedNodeInfo.HasFound && parent is ParagraphNode paragraphNode && parent.RightOriginElement is ParagraphNode)
            {
                transaction.AddCursorPosition(parent.RightOriginElement.Id, paragraphNode.GetTextLength());
                
                transaction.MoveRange(parent.FirstChild, null, parent.RightOriginElement, null);
                transaction.Delete(parent);
            }
            else if (deletedNodeInfo.HasFound)
            {
                transaction.AddCursorPosition(parent.Id, deletedNodeInfo.Offset);
            }
        }
        else
        {
            var nodeSearchResult = DeleteSelection.Handle(keyPressInfo, transaction, parent);
            
            var cursorPosition = nodeSearchResult.Origin.AbsoluteOffsetIfPresent;
            var cursorTarget = nodeSearchResult.Origin.Node?.ParentElement ?? parent;
            if (cursorPosition is not null) transaction.AddCursorPosition(cursorTarget.Id, cursorPosition.Value);
        }
        
        return dScratchService.Apply(transaction);
    }
    
    private static NodeOffset SimpleDeleteForward(KeyPressInfo keyPressInfo, ITransaction transaction, DNode parent)
    {
        var walker = new TreeWalker<TextNode>(parent);
        
        var currentOffset = 0;
        var current = walker.NextNode();
        while (current is not null)
        {
            if (currentOffset + current.Length > keyPressInfo.Selection.AnchorOffset)
            {
                break;
            }

            currentOffset += current.Length;
            current = walker.NextNode();
        }

        var relativeOffset = keyPressInfo.Selection.AnchorOffset - currentOffset;
        var noteToDelete = current is not null ? transaction.SplitText(current, relativeOffset) : null;
        if (noteToDelete is not null)
        {
            transaction.SplitText(noteToDelete, 1);
            transaction.Delete(noteToDelete);
        }

        return NodeOffset.From(noteToDelete, keyPressInfo.Selection.AnchorOffset);
    }
}