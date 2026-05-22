using DScratch.Nodes;
using DScratch.Transactions.Steps;

namespace DScratch.Transactions;

public interface ITransaction
{
    internal TransactionResult Commit();

    ITransaction Insert(DNode node, DNode parent);

    ITransaction DeleteNode(DNode node);

    DNode? FindNode(NodePath path);
}