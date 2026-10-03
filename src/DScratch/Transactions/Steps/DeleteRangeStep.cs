using DScratch.Nodes;
using DScratch.Rendering;

namespace DScratch.Transactions.Steps;

public class DeleteRangeStep(NodeId? start, NodeId? end) : IStep
{
    public IReadOnlyList<StepDiff?> Execute(IRunningTransaction transaction, DScratchDocument document)
    {
        if (start is null && end is null) return [];
        
        var steps = new List<StepDiff?>();

        var startNode = start.HasValue ? document.FindNode(start.Value) : null;
        var endNode = end.HasValue ? document.FindNode(end.Value) : null;

        var nearestBlockStart = startNode?.GetNearestBlock();
        var nearestBlockEnd = endNode?.GetNearestBlock();

        if (nearestBlockStart is not null && nearestBlockEnd is not null && nearestBlockStart != nearestBlockEnd)
        {
            throw new InvalidOperationException("Start and End must be within the same block element.");
        }

        var nearestBlock = nearestBlockStart ?? nearestBlockEnd!;

        DNode firstNodeToDelete;
        if (startNode is TextNode startTextNode)
        {
            var (node, stepDiffs) = SplitStartTextNode(transaction, document, startTextNode, start!.Value);
            steps.AddRange(stepDiffs);
            firstNodeToDelete = node.PreviousSibling()!;
        }
        else
        {
            firstNodeToDelete = startNode ?? nearestBlockEnd!.FirstChild!;
        }

        DNode lastNodeToDelete;
        if (endNode is TextNode endTextNode)
        {
            var (node, stepDiffs) = SplitEndTextNode(transaction, document, endTextNode, start!.Value);
            steps.AddRange(stepDiffs);
            lastNodeToDelete = node;
        }
        else
        {
            lastNodeToDelete = endNode ?? nearestBlockStart!.LastChild!;
        }

        var walker = TreeWalker<DNode>.StartFrom(firstNodeToDelete!, nearestBlock);

        var endParent = GetEndParent(lastNodeToDelete, nearestBlock);
        
        while (walker.Node is not null && walker.Node != endParent)
        {
            walker.Node.Delete();
            steps.Add(walker.Node.ToDeleteSteps());
            transaction.NotifyNodeChange(walker.Node);
            walker.NextNode(true);
        }
        
        while (walker.Node is not null && walker.Node != lastNodeToDelete)
        {
            walker.Node.Delete();
            steps.Add(walker.Node.ToDeleteSteps());
            transaction.NotifyNodeChange(walker.Node);
            walker.NextNode();
        }

        if (walker.Node is null) return steps;
        
        walker.Node.Delete();
        steps.Add(walker.Node.ToDeleteSteps());
        transaction.NotifyNodeChange(walker.Node);
        return steps;
    }

    public IReadOnlyList<StepDiff> Revert(DScratchDocument document)
    {
        throw new NotImplementedException();
    }

    private static DNode GetEndParent(DNode lastNodeToDelete, DNode nearestBlock)
    {
        var current = lastNodeToDelete;
        while (current.Parent != nearestBlock)
        {
            current = current.Parent!;
        }

        return current;
    }
    
    private static (DNode Node, IReadOnlyList<StepDiff?> StepDiffs) SplitStartTextNode(IRunningTransaction transaction, DScratchDocument document, TextNode textNode, NodeId nodeId)
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

        var steps = SplitTextStep.GenerateSplitSteps(transaction, document, textNode, splitNode, offset);
        return (splitNode, steps); // TODO: test scenario
    }
    
    private static (DNode Node, IReadOnlyList<StepDiff?> StepDiffs) SplitEndTextNode(IRunningTransaction transaction, DScratchDocument document, TextNode textNode, NodeId nodeId)
    {
        if (textNode.Length == 1 || textNode.LastId == nodeId) // TODO: test scenario
        {
            return (textNode, []);
        }
        
        var offset = (int)(nodeId.Clock - textNode.Id.Clock + 1);
        var splitNode = textNode.Split(offset, transaction.NodeFactory.NodeIdGenerator.TakeIds);

        if (splitNode is null)
        {
            throw new InvalidOperationException($"NodeId {nodeId} was outside of TextNode range.");
        }

        var steps = SplitTextStep.GenerateSplitSteps(transaction, document, textNode, splitNode, offset);
        return (textNode, steps); // TODO: test scenario
    }
}