using DScratch.Nodes;
using DScratch.Rendering;

namespace DScratch.Transactions.Steps;

public class ReplaceNodeStep(DNode node, Func<DNode, DNode> copyFactory) : IStep
{
    public IReadOnlyList<StepDiff?> Execute(IRunningTransaction transaction, DScratchDocument document)
    {
        var newNode = copyFactory(node);
        newNode.ClaimChildNodes();

        node.Parent!.ReplaceChild(node, newNode);
        
        document.RemoveNode(node);
        document.AddNode(newNode);
        transaction.NotifyNodeChange(newNode);

        return [node.ToDeleteSteps(), ..newNode.ToInsertSteps()];
    }

    public IReadOnlyList<StepDiff?> Revert(DScratchDocument document)
    {
        throw new NotImplementedException();
    }
}