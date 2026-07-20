using DScratch.Interactions.CommandHandlers.Handlers;
using DScratch.Nodes.Marks;

namespace DScratch.Interactions.CommandHandlers.Commands;

public record UpdateMarkCommand(Mark Mark, UpdateMarkAction Action) : IEditorCommand;