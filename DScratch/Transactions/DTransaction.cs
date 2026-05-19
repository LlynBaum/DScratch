using DScratch.Nodes;
using DScratch.Transactions.Steps;

namespace DScratch.Transactions;

public class DTransaction(DScratchDocument document)
{
    private readonly List<IStep> steps = [];
    
    internal TransactionResult Commit()
    {
        var diffs = steps.SelectMany(s => s.Execute(document)).ToList();
        return new TransactionResult(diffs);
    }
    
    public DTransaction Insert(DNode node, NodePath path, int offset)
    {
        steps.Add(new InsertStep(node, path, offset));
        return this;
    }
    
    public DTransaction InsertRange(DNode first, DCharNode last, NodePath path, int offset)
    {
        steps.Add(new InsertRangeStep(first, last, path, offset));
        return this;
    }
    
    public DTransaction DeleteNode(NodePath path, int offset)
    {
        steps.Add(new DeleteStep(path, offset));
        return this;
    }
}