namespace DScratch.Interactions.CommandHandlers.Commands;

public record ChangeBlockTypeCommand(BlockNodeType TargetBlockNodeType) : IEditorCommand;