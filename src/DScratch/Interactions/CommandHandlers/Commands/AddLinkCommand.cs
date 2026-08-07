namespace DScratch.Interactions.CommandHandlers.Commands;

public record AddLinkCommand(string Href) : IEditorCommand;