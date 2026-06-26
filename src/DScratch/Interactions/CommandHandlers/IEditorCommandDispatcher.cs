namespace DScratch.Interactions.CommandHandlers;

public interface IEditorCommandDispatcher
{
    Task ChangeBlockTypeAsync(BlockNodeType targetBlockNodeType);
}