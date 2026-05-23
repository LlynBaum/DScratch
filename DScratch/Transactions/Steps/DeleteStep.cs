using DScratch.Nodes;

namespace DScratch.Transactions.Steps;

public class DeleteStep(DNode node) : IStep
{
    public IReadOnlyList<StepDiff> Execute(DScratchDocument document)
    {
        // must be called before .Delete(), else the node can not be found anymore
        var step = node.ToDeleteSteps();
        node.Delete();
        return [step];
    }

    public IReadOnlyList<StepDiff> Revert(DScratchDocument document)
    {
        throw new NotImplementedException();
    }
}