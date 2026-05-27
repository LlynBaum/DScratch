using DScratch.Client.Scripts.EventHandlers.Common;
using DScratch.Nodes;
using DScratch.Nodes.NodeTypes;
using DScratch.Transactions;

namespace DScratch.Client.Scripts.EventHandlers;

public class DeleteContentBackwardHandler(IDScratchService dScratchService) : IEditorEventHandler
{
    public const string EventName = "deleteContentBackward";
    
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
            // TODO: we can detect this, when simpleDelete has nothing found to delete, then we are at the start of the paragraph
            if (parent.IsParagraphNode() && keyPressInfo.Selection.Offset < 1)
            {
                // so we are at the start of a text element... like a p element... we have to delete it, and move text over to previous element, if possible, else fuck it xD
                throw new NotImplementedException();
            }
            
            SimpleDeleteBackwards(keyPressInfo, transaction, parent);
        }
        else
        {
            DeleteSelection.Handle(keyPressInfo, transaction, parent);
        }
        
        transaction.AddCursorPosition(parent.Id, 0); // TODO: get absolut position. Maybe add a record with all infos like absolut and relative offsets. And SimpleInsert and DeleteSelection and so on will return all those infos always.
        return dScratchService.Apply(transaction);
    }

    private static void SimpleDeleteBackwards(KeyPressInfo keyPressInfo, ITransaction transaction, DNode parent)
    {
        var walker = new TreeWalker<TextNode>(parent);
        
        var currentOffset = 0;
        var current = walker.NextNode();
        while (current is not null)
        {
            var length = current.Length;
            if (currentOffset + length >= keyPressInfo.Selection.Offset)
            {
                break;
            }

            currentOffset += length;
            current = walker.NextNode();
        }

        var nodeToDelete = current?.ChildAt(keyPressInfo.Selection.Offset - currentOffset - 1);
        if (nodeToDelete is not null)
        {
            transaction.Delete(nodeToDelete);
        }
    }
}