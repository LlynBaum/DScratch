namespace DScratch.Transactions;

public interface IStep
{
    IReadOnlyList<StepDiff?> Execute(IRunningTransaction transaction, DScratchDocument document);

    IReadOnlyList<StepDiff?> Revert(DScratchDocument document);
}