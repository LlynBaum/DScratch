namespace DScratch.Transactions;

public interface IStep
{
    IReadOnlyList<StepDiff> Execute();

    IReadOnlyList<StepDiff> Revert(DScratchDocument document);
}