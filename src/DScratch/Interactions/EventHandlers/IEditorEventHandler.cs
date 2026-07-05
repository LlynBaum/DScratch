using DScratch.Transactions;

namespace DScratch.Interactions.EventHandlers;

public interface IEditorEventHandler
{
    void Handle(KeyPressInfo keyPressInfo, ITransaction transaction);
}