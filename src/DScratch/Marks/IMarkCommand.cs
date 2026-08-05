using DScratch.Interactions.CommandHandlers;
using DScratch.Interactions.UserStates;
using DScratch.Nodes;
using DScratch.Transactions;

namespace DScratch.Marks;

public interface IMarkCommand : IEditorCommand
{
    void Execute(ITransaction transaction, DNode anchor, IEnumerable<DNode> nodes);

    void AddPending(IUserStateService userStateService);
}