using DScratch.Marks;
using DScratch.Nodes;
using DScratch.Rendering;

namespace DScratch.Transactions.Steps.Marks;

public class RemoveMarkStep(DNode node, MarkKey key) : IStep
{
    public IReadOnlyList<StepDiff?> Execute(IRunningTransaction transaction, DScratchDocument document)
    {
        node.RemoveMark(key);
        transaction.NotifyNodeChange(node);
        return [node.ToMarkUpdate()];
    }

    public IReadOnlyList<StepDiff?> Revert(DScratchDocument document)
    {
        throw new NotImplementedException();
    }
}