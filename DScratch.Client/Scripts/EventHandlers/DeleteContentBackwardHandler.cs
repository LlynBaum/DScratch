using DScratch.Transactions.Steps;

namespace DScratch.Client.Scripts.EventHandlers;

public class DeleteContentBackwardHandler(IDScratchService dScratchService) : IEditorEventHandler
{
    public const string EventName = "deleteContentBackward";
    
    public TransactionResult Handle(KeyPressInfo keyPressInfo, DScratchDocument document)
    {
        var transaction = dScratchService.StartTransaction(document);
        transaction.DeleteNode(keyPressInfo.GetNodePath(), keyPressInfo.Selection.Offset);
        return dScratchService.Apply(transaction);
    }
}