using DScratch.Transactions;

namespace DScratch.Interactions.CommandHandlers;

public abstract class CommandBase<TCommand>(IDScratchService dScratchService) : IEditorCommandHandler<TCommand> where TCommand : IEditorCommand
{
    public TransactionResult Execute(SelectionInfo selectionInfo, TCommand command)
    {
        var transaction = dScratchService.StartTransaction();
        Handle(transaction, selectionInfo, command);
        return dScratchService.Apply(transaction);
    }

    protected abstract void Handle(ITransaction transaction, SelectionInfo selectionInfo, TCommand command);
}