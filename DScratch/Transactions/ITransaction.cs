using DScratch.Nodes;
using DScratch.Transactions.Steps;

namespace DScratch.Transactions;

public interface ITransaction
{
    DNode Root { get; }
    
    internal TransactionResult Commit();

    ITransaction Insert(DNode node, DNode parent);

    ITransaction Delete(DNode node);

    DNode? FindNode(NodePath path);
}