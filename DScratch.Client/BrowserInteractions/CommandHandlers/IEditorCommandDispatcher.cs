namespace DScratch.Client.BrowserInteractions.CommandHandlers;

public interface IEditorCommandDispatcher
{
    Task ChangeBlockTypeAsync(BlockNodeType targetBlockNodeType);
}