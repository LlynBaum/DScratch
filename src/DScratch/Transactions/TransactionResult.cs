using DScratch.Interactions;

namespace DScratch.Transactions;

internal sealed record TransactionResult(IReadOnlyList<ModifiedNode> ModifiedNodes, SelectionInfo? CursorPosition = null)
{
    public bool IsEmpty => !ModifiedNodes.Any();
}