namespace DScratch.Transactions;

public sealed record CursorPosition(string ParentId, int Offset);

public sealed record TransactionResult(IReadOnlyList<StepDiff?> Steps, CursorPosition? CursorPosition = null)
{
    public bool IsEmpty => !Steps.Any();
}