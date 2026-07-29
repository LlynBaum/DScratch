using DScratch.Nodes;
using DScratch.Nodes.Marks;
using DScratch.Rendering;

namespace DScratch.Transactions.Steps.Marks;

public class AddMarkStep(DNode node, MarkKey key, string value) : IStep
{
    public IReadOnlyList<StepDiff?> Execute(IRunningTransaction transaction, DScratchDocument document)
    {
        node.SetMark(key, value);
        transaction.NotifyNodeChange(node);
        return [node.ToMarkUpdate()];
    }

    public IReadOnlyList<StepDiff?> Revert(DScratchDocument document)
    {
        throw new NotImplementedException();
    }
}