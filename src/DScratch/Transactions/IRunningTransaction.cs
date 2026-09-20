using DScratch.Nodes;

namespace DScratch.Transactions;

public interface IRunningTransaction
{
    INodeFactory NodeFactory { get; }
    
    void NotifyNodeChange(DNode node);
}