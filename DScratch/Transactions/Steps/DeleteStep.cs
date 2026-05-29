using DScratch.Nodes;

namespace DScratch.Transactions.Steps;

public class DeleteStep(DNode node) : IStep
{
    public IReadOnlyList<StepDiff?> Execute()
    {
        node.Delete();
        return [node.ToDeleteSteps()];
    }

    public IReadOnlyList<StepDiff> Revert(DScratchDocument document)
    {
        throw new NotImplementedException();
    }
}