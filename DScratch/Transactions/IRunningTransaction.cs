using DScratch.Nodes;

namespace DScratch.Transactions;

public interface IRunningTransaction
{
    void NotifyNodeChange(DNode node);
}