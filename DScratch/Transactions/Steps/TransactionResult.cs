namespace DScratch.Transactions.Steps;

public record TransactionResult(IReadOnlyList<StepDiff?> Diffs)
{
    public static TransactionResult Empty => new TransactionResult([]);
    
    public bool IsEmpty => !Diffs.Any();
}