using DScratch.Interactions.CommandHandlers.Handlers;
using DScratch.Nodes.Marks;

namespace DScratch.Interactions.CommandHandlers.Commands;

public record UpdateMarkCommand : IEditorCommand
{
    private UpdateMarkCommand(MarkKey Key, string? Value, UpdateMarkAction Action)
    {
        this.Key = Key;
        this.Value = Value;
        this.Action = Action;
    }

    public MarkKey Key { get; init; }
    public string? Value { get; init; }
    public UpdateMarkAction Action { get; init; }

    public static UpdateMarkCommand Add(MarkKey key, string value)
    {
        return new UpdateMarkCommand(key, value, UpdateMarkAction.Add);
    }
    
    public static UpdateMarkCommand Remove(MarkKey key)
    {
        return new UpdateMarkCommand(key, null, UpdateMarkAction.Remove);
    }
    
    public static UpdateMarkCommand Toggle(MarkKey key, string value)
    {
        return new UpdateMarkCommand(key, value, UpdateMarkAction.Toggle);
    }
}