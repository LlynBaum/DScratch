using DScratch.Nodes;

namespace DScratch.Transactions.Steps;

public class UpdateNodeTypeStep(DNode node, Func<DNode, DNode> copyFactory) : IStep
{
    public IReadOnlyList<StepDiff?> Execute(IRunningTransaction transaction, DScratchDocument document)
    {
        var parent = node.Parent!;
        node.Remove();

        var newNode = copyFactory(node);
        parent.InsertChild(newNode);
        
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