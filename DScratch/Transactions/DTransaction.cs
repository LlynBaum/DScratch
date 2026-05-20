using DScratch.Nodes;
using DScratch.Transactions.Steps;

namespace DScratch.Transactions;

internal class DTransaction(DScratchDocument document) : ITransaction
{
    private readonly List<IStep> steps = [];
    
    public TransactionResult Commit()
    {
        var diffs = steps.SelectMany(s => s.Execute(document)).ToList();
        return new TransactionResult(diffs);
    }
    
    public ITransaction Insert(DNode node, NodePath path, int offset)
    {
        steps.Add(new InsertStep(node, path, offset));
        return this;
    }
    
    public ITransaction DeleteNode(NodePath path, int offset)
    {
        steps.Add(new DeleteStep(path, offset));
        return this;
    }
}