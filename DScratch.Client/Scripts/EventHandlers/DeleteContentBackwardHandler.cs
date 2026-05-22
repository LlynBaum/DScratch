using DScratch.Transactions.Steps;

namespace DScratch.Client.Scripts.EventHandlers;

public class DeleteContentBackwardHandler(IDScratchService dScratchService) : IEditorEventHandler
{
    public const string EventName = "deleteContentBackward";
    
    public TransactionResult Handle(KeyPressInfo keyPressInfo, DScratchDocument document)
    {
        var transaction = dScratchService.StartTransaction(document);
        
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
        
        var nodeToDelete = parent.ChildAt(keyPressInfo.Selection.Offset - 1);
        if (nodeToDelete is null) return TransactionResult.Empty;
        
        transaction.DeleteNode(nodeToDelete);
        return dScratchService.Apply(transaction);
    }
}