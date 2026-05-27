namespace DScratch.Transactions;

public sealed record CursorPosition(string[] Parent, int Offset);

public sealed record TransactionResult(IReadOnlyList<StepDiff?> Steps, CursorPosition? CursorPosition = null)
{
    public static TransactionResult Empty => new TransactionResult([], null);
    
    public bool IsEmpty => !Steps.Any();
}