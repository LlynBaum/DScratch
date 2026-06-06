using DScratch.Client.BrowserInteractions.EventHandlers.Common;
using DScratch.Client.BrowserInteractions.EventHandlers.Models;
using DScratch.Nodes;
using DScratch.Transactions;

namespace DScratch.Client.BrowserInteractions.EventHandlers;

public class DeleteContentBackwardHandler(IDScratchService dScratchService) : IEditorEventHandler
{
    public const string EventName = "deleteContentBackward";
    
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
            var deletedNodeInfo = SimpleDeleteBackwards(keyPressInfo, transaction, parent);

            if (!deletedNodeInfo.HasFound && parent is ParagraphNode && parent.OriginElement is ParagraphNode paragraphNode) // TODO: probably just BLockElements in general
            {
                transaction.AddCursorPosition(parent.OriginElement.Id, paragraphNode.GetTextLength());
                
                transaction.MoveRange(parent.FirstChild, null, parent.OriginElement, parent.OriginElement.LastChild);
                transaction.Delete(parent);
            }
            else if (deletedNodeInfo.HasFound) // TODO: add a case for inline elements, that will have the same without the merging
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

    private static NodeOffset SimpleDeleteBackwards(KeyPressInfo keyPressInfo, ITransaction transaction, DNode parent)
    {
        var walker = new TreeWalker<TextNode>(parent);
        
        var currentOffset = 0;
        var current = walker.NextNode();
        while (current is not null)
        {
            var length = current.Length;
            if (currentOffset + length >= keyPressInfo.Selection.AnchorOffset)
            {
                break;
            }

            currentOffset += length;
            current = walker.NextNode();
        }

        var relativeOffset = keyPressInfo.Selection.AnchorOffset - currentOffset - 1;
        var nodeToDelete = current?.ChildAt(relativeOffset);
        if (nodeToDelete is not null)
        {
            transaction.Delete(nodeToDelete);
        }
        
        return NodeOffset.From(nodeToDelete, keyPressInfo.Selection.AnchorOffset - 1);
    }
}