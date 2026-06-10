namespace DScratch.Transactions;

public interface IStep
{
    IReadOnlyList<StepDiff?> Execute(IRunningTransaction transaction);

    IReadOnlyList<StepDiff?> Revert(DScratchDocument document);
}