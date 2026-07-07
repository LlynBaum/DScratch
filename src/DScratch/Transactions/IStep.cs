namespace DScratch.Transactions;

public interface IStep
{
    void Execute(IRunningTransaction transaction, DScratchDocument document);

    void Revert(DScratchDocument document);
}