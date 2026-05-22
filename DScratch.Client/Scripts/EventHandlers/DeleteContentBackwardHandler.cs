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
        
        var offset = parent.ActiveChildNodes[keyPressInfo.Selection.Offset];
        
        
        transaction.DeleteNode();
        return dScratchService.Apply(transaction);
    }
}