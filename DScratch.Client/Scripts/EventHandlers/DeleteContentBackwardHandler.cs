namespace DScratch.Client.Scripts.EventHandlers;

public class DeleteContentBackwardHandler : IEditorEventHandler
{
    public const string EventName = "deleteContentBackward";
    
    public void Handle(KeyPressInfo keyPressInfo, DScratchDocument document)
    {
        var transaction = new DTransaction(document);
        transaction.DeleteNode(keyPressInfo.GetNodePath(), keyPressInfo.Selection.Offset);
    }
}