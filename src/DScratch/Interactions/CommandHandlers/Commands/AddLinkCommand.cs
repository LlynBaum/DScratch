namespace DScratch.Interactions.CommandHandlers.Commands;

public record AddLinkCommand(string Href, string Target, string? DisplayText = null) : IEditorCommand;