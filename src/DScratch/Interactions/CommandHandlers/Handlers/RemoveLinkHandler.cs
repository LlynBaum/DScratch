using DScratch.Interactions.CommandHandlers.Commands;
using DScratch.Transactions;

namespace DScratch.Interactions.CommandHandlers.Handlers;

public class RemoveLinkHandler(IDScratchService dScratchService) : CommandBase<RemoveLinkCommand>(dScratchService)
{
    protected override void Handle(ITransaction transaction, SelectionInfo selectionInfo, RemoveLinkCommand command)
    {
        throw new NotImplementedException();
    }
}