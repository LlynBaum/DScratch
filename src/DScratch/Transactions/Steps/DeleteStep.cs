using DScratch.Nodes;
using DScratch.Rendering;

namespace DScratch.Transactions.Steps;

public class DeleteStep(NodeId nodeId) : IStep
{
    public IReadOnlyList<StepDiff?> Execute(IRunningTransaction transaction, DScratchDocument document)
    {
        var node = document.FindNode(nodeId) ?? throw new ArgumentException($"Could not find node {nodeId}");

        var stepDiffs = new List<StepDiff?>();
        if (node is TextNode textNode)
        {
            var result = SplitTextNode(transaction, document, textNode, nodeId);
            node = result.Node;
            stepDiffs.AddRange(result.StepDiffs);
            
        }
        
        node.Delete();
        transaction.NotifyNodeChange(node);
        stepDiffs.Add(node.ToDeleteSteps());
        return stepDiffs;
    }

    private static (DNode Node, IReadOnlyList<StepDiff?> StepDiffs) SplitTextNode(IRunningTransaction transaction, DScratchDocument document, TextNode textNode, NodeId nodeId)
    {
        if (textNode.Length == 1) // TODO: test scenario
        {
            return (textNode, []);
        }
        
        var offset = (int)(nodeId.Clock - textNode.Id.Clock);
        var splitNode = textNode.Split(offset, transaction.NodeFactory.NodeIdGenerator.TakeIds);

        if (splitNode is null)
        {
            throw new InvalidOperationException($"NodeId {nodeId} was outside of TextNode range.");
        }

        var steps1 = SplitTextStep.GenerateSplitSteps(transaction, document, textNode, splitNode, offset);
        
        if (splitNode.Length == 1) // TODO: test scenario
        {
            return (splitNode, steps1);
        }

        var splitNode2 = splitNode.Split(1, transaction.NodeFactory.NodeIdGenerator.TakeIds);
        var steps2 = splitNode2 is not null 
            ? SplitTextStep.GenerateSplitSteps(transaction, document, splitNode, splitNode2, 1) 
            : [];

        return (splitNode, [.. steps1, .. steps2]);
    }

    public IReadOnlyList<StepDiff> Revert(DScratchDocument document)
    {
        throw new NotImplementedException();
    }
}