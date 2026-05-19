namespace DScratch.Transactions;

public interface IStep
{
    StepDiff Execute(DScratchDocument document);

    StepDiff Revert(DScratchDocument document);

    public abstract record StepDiff(StepType StepType);
    
    public enum StepType
    {
        Insert,
        InsertRange,
        Delete
    }
}