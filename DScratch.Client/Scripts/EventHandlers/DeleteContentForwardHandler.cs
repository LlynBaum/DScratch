using DScratch.Client.Scripts.EventHandlers.Common;
using DScratch.Nodes;
using DScratch.Transactions;
using DScratch.Transactions.Steps;

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

        if (keyPressInfo.Selection.Direction is SelectionDirection.None)
        {
            if (keyPressInfo.Selection.Offset >= 10) // TODO when selection is at the end of the paragraph or element
            {
                // so we are at the start of a text element... like a p element... we have to delete it, and move text over to previous element, if possible, else fuck it xD
                throw new NotImplementedException();
            }
            
            SimpleDeleteForward(keyPressInfo, transaction, parent);
        }
        else
        {
            // TODO: deleting over two paragraphs will be more complex. Need to merge them together in that case...
            DeleteSelection.Handle(keyPressInfo, transaction, parent);
        }
        
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