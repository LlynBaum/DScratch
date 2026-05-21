using DScratch.Transactions.Steps;

namespace DScratch.Client.Scripts.EventHandlers;

public class InsertTextHandler(IDScratchService dScratchService) : IEditorEventHandler
{
    public const string EventName = "insertText";
    
    public TransactionResult Handle(KeyPressInfo keyPressInfo, DScratchDocument document)
    {
        if (keyPressInfo.Data is null)
        {
            return TransactionResult.Empty;
        }
        
        var transaction = dScratchService.StartTransaction(document);
        var textNode = dScratchService.NodeFactory.String(keyPressInfo.Data);
        transaction.Insert(textNode, keyPressInfo.GetNodePath(), keyPressInfo.Selection.Offset);
        return dScratchService.Apply(transaction);
    }
}