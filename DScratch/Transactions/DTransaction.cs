using DScratch.Nodes;
using DScratch.Transactions.Steps;

namespace DScratch.Transactions;

internal class DTransaction(DScratchDocument document) : ITransaction
{
    private readonly List<IStep> steps = [];

    public IReadOnlyList<IStep> Steps => steps;

    public DNode Root => document.Page.Root;

    public TransactionResult Commit()
    {
        var diffs = steps.SelectMany(s => s.Execute(document)).ToList();
        return new TransactionResult(diffs);
    }
    
    public ITransaction Insert(DNode node, DNode parent)
    {
        steps.Add(new InsertStep(node, parent));
        return this;
    }
    
    public ITransaction Delete(DNode node)
    {
        steps.Add(new DeleteStep(node));
        return this;
    }

    public DNode? FindNode(NodePath path) => document.FindNode(path);
}