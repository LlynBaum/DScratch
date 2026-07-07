namespace DScratch.Transactions;

public interface IRunningTransaction
{
    void NotifyNodeChange(ModifiedNode modifiedNode);
}