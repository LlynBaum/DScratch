using DScratch.Nodes;

namespace DScratch.Transactions.Steps;

public class DeleteStep(DNode node) : IStep
{
    public IReadOnlyList<StepDiff?> Execute()
    {
        // TODO: when deleting text, we can combine continues tombstones
        node.Delete();
        return [node.ToDeleteSteps()];
    }

    public IReadOnlyList<StepDiff> Revert(DScratchDocument document)
    {
        throw new NotImplementedException();
    }
}