using DScratch.Nodes;
using DScratch.Rendering;

namespace DScratch.Transactions.Steps;

internal class InsertStep(DNode node, DNode parent) : IStep
{
    public IReadOnlyList<StepDiff?> Execute(IRunningTransaction transaction, DScratchDocument document)
    {
        var splitSteps = EnsureOriginExists(transaction, document);
        parent.InsertChild(node);
        transaction.NotifyNodeChange(node);
        document.AddNode(node);
        return [..splitSteps, ..node.ToInsertSteps()];
    }

    private IReadOnlyList<StepDiff?> EnsureOriginExists(IRunningTransaction transaction, DScratchDocument document)
    {
        if (!node.Origin.HasValue)
        {
            return [];
        }
        
        var originNode = document.FindNode(node.Origin.Value);
        if (originNode is null)
        {
            throw new ArgumentException($"Could not find node {node.Origin}");
        }

        if (originNode.Parent != parent)
        {
            throw new InvalidOperationException($"Node {originNode.Id} has Parent {originNode.Parent?.Id} but expected {parent.Id}");
        }

        if (originNode.LastId == node.Origin || originNode is not TextNode textNode)
        {
            return [];
        }

        var offset = (int)(node.Origin.Value.Clock - textNode.Id.Clock + 1);
        var splitNode = textNode.Split(offset, transaction.NodeFactory.NodeIdGenerator.TakeIds);
        return splitNode is not null 
            ? SplitTextStep.GenerateSplitSteps(document, textNode, splitNode, offset)
            : [];
    }

    public IReadOnlyList<StepDiff> Revert(DScratchDocument document)
    {
        throw new NotImplementedException();
    }
}