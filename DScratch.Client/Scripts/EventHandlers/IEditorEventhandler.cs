using DScratch.Transactions;
using DScratch.Transactions.Steps;

namespace DScratch.Client.Scripts.EventHandlers;

public interface IEditorEventHandler
{
    TransactionResult Handle(KeyPressInfo keyPressInfo);
}