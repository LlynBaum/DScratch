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

        var parent = transaction.FindNode(keyPressInfo.GetNodePath());
        if (parent is null)
        {
            throw new ArgumentException($"Parent with given path not found: {keyPressInfo.GetNodePath()}");
        }

        var rightOrigin = parent.ChildAt(keyPressInfo.Selection.Offset);
        var origin = parent.ChildAt(keyPressInfo.Selection.Offset - 1);
        
        var textNode = dScratchService.NodeFactory.String(keyPressInfo.Data, origin, rightOrigin);
        transaction.Insert(textNode, parent);
        return dScratchService.Apply(transaction);
    }
}