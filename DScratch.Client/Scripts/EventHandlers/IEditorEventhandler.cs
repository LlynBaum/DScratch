using DScratch.Transactions;

namespace DScratch.Client.Scripts.EventHandlers;

public interface IEditorEventHandler
{
    DTransaction? Handle(KeyPressInfo keyPressInfo, DScratchDocument document);
}