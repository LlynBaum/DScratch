using DScratch.Nodes.NodeTypes;
using DScratch.Transactions.Steps;

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

        if (keyPressInfo.Selection.Offset < 1)
        {
            // so we are at the start of a text element... like a p element... we have to delete it, and move text over to previous element, if possible, else fuck it xD
            throw new NotImplementedException();
        }
        
        // TODO: when selection is not just cursor position, but a selection, then delete everything that is selected.
        
        var currentOffset = 0;
        var walker = new TreeWalker<IShowText>(parent);
        var current = walker.NextNode();
        
        while (current is not null)
        {
            var length = current.Length; // Some Length ops can be more expensive, so avoid calling multiple times
            if (currentOffset + length >= keyPressInfo.Selection.Offset)
            {
                break;
            }

            currentOffset += length;
            current = walker.NextNode();
        }

        var nodeToDelete = walker.Current?.ChildAt(keyPressInfo.Selection.Offset - currentOffset);
        if (nodeToDelete is null) return TransactionResult.Empty;
        
        transaction.DeleteNode(nodeToDelete);
        return dScratchService.Apply(transaction);
    }
}