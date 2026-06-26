using DScratch.Transactions;

namespace DScratch.Interactions.EventHandlers;

public interface IEditorEventHandler
{
    TransactionResult Handle(KeyPressInfo keyPressInfo);
}