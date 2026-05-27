using DScratch.Client.Scripts.EventHandlers.Common;
using DScratch.Nodes;
using DScratch.Nodes.NodeTypes;
using DScratch.Transactions;

namespace DScratch.Client.Scripts.EventHandlers;

public class DeleteContentForwardHandler(IDScratchService dScratchService) : IEditorEventHandler
{
    public const string EventName = "deleteContentForward";
    
    public TransactionResult Handle(KeyPressInfo keyPressInfo)
    {
        var transaction = dScratchService.StartTransaction();
        
        var parent = transaction.FindNode(keyPressInfo.GetNodePath());
        if (parent is null)
        {
            throw new ArgumentException($"Parent with given path not found: {keyPressInfo.GetNodePath()}");
        }
        
        // TODO: when selection is not just cursor position, but a selection, then delete everything that is selected.

        int cursorPosition;
        if (keyPressInfo.Selection.Direction is SelectionDirection.None)
        {
            // TODO: we can detect this, when simpleDelete has nothing found to delete, then we are at the end of the paragraph
            if (parent.IsParagraphNode() && keyPressInfo.Selection.Offset >= 10)
            {
                // so we are at the start of a text element... like a p element... we have to delete it, and move text over to previous element, if possible, else fuck it xD
                throw new NotImplementedException();
            }
            
            SimpleDeleteForward(keyPressInfo, transaction, parent);
            cursorPosition = keyPressInfo.Selection.Offset - 1;
        }
        else
        {
            var nodeSearchResult = DeleteSelection.Handle(keyPressInfo, transaction, parent);
            cursorPosition = nodeSearchResult.Origin?.AbsolutOffset ?? 0;
        }
        
        transaction.AddCursorPosition(parent.Id, cursorPosition);
        return dScratchService.Apply(transaction);
    }
    
    private static void SimpleDeleteForward(KeyPressInfo keyPressInfo, ITransaction transaction, DNode parent)
    {
        var walker = new TreeWalker<TextNode>(parent);
        
        var currentOffset = 0;
        var current = walker.NextNode();
        while (current is not null)
        {
            var length = current.Length;
            if (currentOffset + length > keyPressInfo.Selection.Offset)
            {
                break;
            }

            currentOffset += length;
            current = walker.NextNode();
        }

        var nodeToDelete = current?.ChildAt(keyPressInfo.Selection.Offset - currentOffset);
        if (nodeToDelete is not null)
        {
            transaction.Delete(nodeToDelete);
        }
    }
}