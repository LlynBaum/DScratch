using DScratch.Transactions;

namespace DScratch.Client.BrowserInteractions.EventHandlers;

public class DeleteWordForwardHandler(IDScratchService dScratchService) : IEditorEventHandler
{
    public const string EventName = "deleteWordForward";
    
    public TransactionResult Handle(KeyPressInfo keyPressInfo)
    {
        
    }
}