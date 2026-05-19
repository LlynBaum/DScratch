using DScratch.Nodes;
using DScratch.Transactions.Steps;

namespace DScratch.Transactions;

public class DTransaction(DScratchDocument document)
{
    private readonly List<IStep> steps = [];
    
    // TODO: store steps that have been taken, then commit all in one go. Like a transaction does, instead of instant change like right now.
    // TODO: steps can produce a diff for the dom
    // TODO: history of past transaction, so things like ctrl-z can be possible.

    internal IReadOnlyList<IStep.StepDiff> Commit()
    {
        return steps.Select(s => s.Execute(document)).ToList();
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