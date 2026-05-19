namespace DScratch.Transactions.Steps;

public record TransactionResult(IReadOnlyList<StepDiff> Diffs);