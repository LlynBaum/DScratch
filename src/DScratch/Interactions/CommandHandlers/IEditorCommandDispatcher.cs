using DScratch.Interactions.CommandHandlers.Commands;
using DScratch.Nodes.Marks;

namespace DScratch.Interactions.CommandHandlers;

public interface IEditorCommandDispatcher
{
    Task ChangeBlockTypeAsync(BlockNodeType targetBlockNodeType);

    Task UpdateMarkAsync(Mark mark, UpdateMarkAction action);
}