namespace DScratch.Client.BrowserInteractions.CommandHandlers;

public interface IEditorCommandDispatcher
{
    Task ChangeBlockTypeAsync(BlockType targetBlockType);
}