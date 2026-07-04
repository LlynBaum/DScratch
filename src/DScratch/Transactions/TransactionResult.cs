using DScratch.Interactions;
using DScratch.Nodes;

namespace DScratch.Transactions;

public sealed record TransactionResult(IReadOnlyList<DNode> ModifiedNodes, SelectionInfo? CursorPosition = null)
{
    public bool IsEmpty => !ModifiedNodes.Any();
}