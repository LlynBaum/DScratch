namespace DScratch.Interactions.CommandHandlers.Commands;

public record UpdateLinkCommand(string? Href, string? Target) : IEditorCommand;