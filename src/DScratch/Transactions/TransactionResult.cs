using DScratch.Interactions;

namespace DScratch.Transactions;

public sealed record TransactionResult(IReadOnlyList<StepDiff?> Steps, SelectionInfo? CursorPosition = null)
{
    public bool IsEmpty => !Steps.Any();
}