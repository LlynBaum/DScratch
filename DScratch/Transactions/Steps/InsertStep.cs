using DScratch.Nodes;

namespace DScratch.Transactions.Steps;

internal class InsertStep(DNode node, DNode parent) : IStep
{
    public IReadOnlyList<StepDiff> Execute(DScratchDocument document)
    {
        parent.InsertChild(node);
        return node.ToInsertSteps();
    }

    public IReadOnlyList<StepDiff> Revert(DScratchDocument document)
    {
        throw new NotImplementedException();
    }
}