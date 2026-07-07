using DScratch.Interactions;

namespace DScratch.Transactions;

public sealed record TransactionResult(IReadOnlySet<ModifiedNode> ModifiedNodes, SelectionInfo? CursorPosition = null)
{
    public bool IsEmpty => !ModifiedNodes.Any();
}