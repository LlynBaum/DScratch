using DScratch.Nodes;
using DScratch.Transactions.Steps;

namespace DScratch.Transactions;

public interface ITransaction
{
    internal TransactionResult Commit();

    ITransaction Insert(DNode node, NodePath path, int offset);

    ITransaction DeleteNode(NodePath path, int offset);
}