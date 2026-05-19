using DScratch.Transactions;

namespace DScratch.Client.Scripts.EventHandlers;

public class DeleteContentBackwardHandler : IEditorEventHandler
{
    public const string EventName = "deleteContentBackward";
    
    public DTransaction Handle(KeyPressInfo keyPressInfo, DScratchDocument document)
    {
        var transaction = new DTransaction(document);
        transaction.DeleteNode(keyPressInfo.GetNodePath(), keyPressInfo.Selection.Offset);
        return transaction;
    }
}