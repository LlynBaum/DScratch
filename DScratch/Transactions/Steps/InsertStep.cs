using DScratch.Nodes;

namespace DScratch.Transactions.Steps;

internal class InsertStep(DNode node, NodePath path, int offset) : IStep
{
    public IStep.StepDiff Execute(DScratchDocument document)
    {
        var parent = document.FindNode(path);
        if (parent is null)
        {
            throw new ArgumentException("Could not find parent Node at the expected path.");
        }
        
        var origin = parent.GetChild(offset - 1);
        var rightOrigin = parent.GetChild(offset);

        node.Origin = origin;
        node.RightOrigin = rightOrigin;
        node.Parent = parent;
        
        parent.InsertChild(node);

        return new InsertDiff(path, offset);
    }

    public IStep.StepDiff Revert(DScratchDocument document)
    {
        throw new NotImplementedException();
    }

    public record InsertDiff(NodePath Path, int Offset) : IStep.StepDiff(IStep.StepType.Insert);
}