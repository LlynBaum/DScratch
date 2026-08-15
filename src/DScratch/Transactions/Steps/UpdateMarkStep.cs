using DScratch.Marks;
using DScratch.Nodes;
using DScratch.Rendering;

namespace DScratch.Transactions.Steps;

public class UpdateMarkStep(DNode node, MarkKey key, string value) : IStep
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