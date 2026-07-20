using DScratch.Transactions;

namespace DScratch.Interactions.CommandHandlers;

public interface IEditorCommandHandler<in TCommand> where TCommand : IEditorCommand
{
    TransactionResult Execute(SelectionInfo selectionInfo, TCommand command);
}