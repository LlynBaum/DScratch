namespace DScratch.Transactions.Steps;

public record TransactionResult(IReadOnlyList<IStep.StepDiff> Diffs);