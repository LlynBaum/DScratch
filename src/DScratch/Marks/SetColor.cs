using DScratch.Interactions.UserStates;
using DScratch.Nodes;
using DScratch.Transactions;

namespace DScratch.Marks;

public class SetColor(string color) : IMarkCommand
{
    public void Execute(ITransaction transaction, DNode anchor, IEnumerable<DNode> nodes)
    {
        foreach (var node in nodes)
        {
            transaction.AddMark(node, MarkKey.Color, color);
        }
    }

    public void ExecutePending(IUserStateService userStateService)
    {
        userStateService.AddPendingMark(MarkKey.Color, color);
    }
}