namespace DScratch.Client.Scripts.EventHandlers;

public class InsertTextHandler(DNodeFactory nodeFactory) : IEditorEventHandler
{
    public const string EventName = "insertText";
    
    public void Handle(KeyPressInfo keyPressInfo, DScratchDocument document)
    {
        if (keyPressInfo.Data is null)
        {
            return;
        }
        
        var transaction = new DTransaction(document);
        var (first, last) = nodeFactory.String(keyPressInfo.Data);
        transaction.InsertRange(first, last, keyPressInfo.GetNodePath(), keyPressInfo.Selection.Offset);
    }
}