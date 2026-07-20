namespace DScratch.Interactions.CommandHandlers;

public interface IEditorCommandDispatcher
{
    Task DispatchAsync<TCommand>(TCommand command) where TCommand : IEditorCommand;
}