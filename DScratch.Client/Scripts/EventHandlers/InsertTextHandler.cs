using DScratch.Transactions;

namespace DScratch.Client.Scripts.EventHandlers;

public class InsertTextHandler(DNodeFactory nodeFactory) : IEditorEventHandler
{
    public const string EventName = "insertText";
    
    public DTransaction? Handle(KeyPressInfo keyPressInfo, DScratchDocument document)
    {
        if (keyPressInfo.Data is null)
        {
            return null;
        }
        
        var transaction = new DTransaction(document);
        var textNode = nodeFactory.String(keyPressInfo.Data);
        transaction.Insert(textNode, keyPressInfo.GetNodePath(), keyPressInfo.Selection.Offset);
        return transaction;
    }
}