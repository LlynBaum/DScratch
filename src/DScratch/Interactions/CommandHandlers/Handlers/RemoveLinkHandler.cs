using DScratch.Interactions.CommandHandlers.Commands;
using DScratch.Transactions;

namespace DScratch.Interactions.CommandHandlers.Handlers;

public class RemoveLinkHandler : IEditorCommandHandler<RemoveLinkCommand>
{
    public TransactionResult Execute(SelectionInfo selectionInfo, RemoveLinkCommand command)
    {
        return new TransactionResult([]); // TODO
    }
}