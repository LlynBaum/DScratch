namespace DScratch.Transactions;

public interface IStep
{
    IReadOnlyList<StepDiff> Execute(DScratchDocument document);

    IReadOnlyList<StepDiff> Revert(DScratchDocument document);
}