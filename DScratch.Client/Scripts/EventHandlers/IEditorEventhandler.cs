namespace DScratch.Client.Scripts.EventHandlers;

public interface IEditorEventHandler
{
    void Handle(KeyPressInfo keyPressInfo, DScratchDocument document);
}