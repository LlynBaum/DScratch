using DScratch.Transactions;

namespace DScratch.Client.BrowserInteractions.EventHandlers;

public interface IEditorEventHandler
{
    TransactionResult Handle(KeyPressInfo keyPressInfo);
}