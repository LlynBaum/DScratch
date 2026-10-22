using DScratch.Nodes;
using DScratch.Nodes.NodeTypes;
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

        // if the start or end is the parent of the other node, we know that we can just delete from that node until the end of that parent.
        if (startNode?.Parent == endNode)
        {
            endNode = null;
        }

        if (endNode?.Parent == startNode)
        {
            startNode = null;
        }

        var nearestBlockStart = startNode is IBlockElement ? startNode : startNode?.GetNearestBlock();
        var nearestBlockEnd = endNode is IBlockElement ? endNode : endNode?.GetNearestBlock();

        if (nearestBlockStart is not null && nearestBlockEnd is not null && nearestBlockStart != nearestBlockEnd)
        {
            throw new InvalidOperationException("Start and End must be within the same block element.");
        }

        var nearestBlock = nearestBlockStart ?? nearestBlockEnd!;

        DNode firstNodeToDelete;
        if (startNode is TextNode startTextNode)
        {
            var (node, stepDiffs) = SplitTextNode(transaction, document, startTextNode, start!.Value);
            steps.AddRange(stepDiffs);
            firstNodeToDelete = node;

            if (endNode is not null && endNode.Id == startTextNode.Id)
            {
                endNode = node;
            }
        }
        else
        {
            firstNodeToDelete = startNode ?? nearestBlockEnd!.FirstChild!;
        }

        DNode? lastNodeToDelete;
        if (endNode is TextNode endTextNode)
        {
            var (node, stepDiffs) = SplitTextNode(transaction, document, endTextNode, end!.Value);
            steps.AddRange(stepDiffs);
            lastNodeToDelete = node;
        }
        else
        {
            lastNodeToDelete = endNode;
        }

        var walker = TreeWalker<DNode>.StartFrom(firstNodeToDelete!, nearestBlock);

        var endParent = lastNodeToDelete is not null 
            ? GetEndParent(lastNodeToDelete, nearestBlock)
            : firstNodeToDelete.Parent;
        
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

        return steps;
    }

    public IReadOnlyList<StepDiff> Revert(DScratchDocument document)
    {
        throw new NotImplementedException();
    }

    private static DNode GetEndParent(DNode lastNodeToDelete, DNode nearestBlock)
    {
        if (lastNodeToDelete == nearestBlock) return lastNodeToDelete;
        
        var current = lastNodeToDelete;
        while (current.Parent != nearestBlock)
        {
            current = current.Parent!;
        }

        return current;
    }
    
    private static (DNode Node, IReadOnlyList<StepDiff?> StepDiffs) SplitTextNode(IRunningTransaction transaction, DScratchDocument document, TextNode textNode, NodeId nodeId)
    {
        if (textNode.Length == 1)
        {
            return (textNode, []);
        }
        
        var offset = (int)(nodeId.Clock - textNode.Id.Clock);
        var splitNode = textNode.Split(offset);

        if (splitNode is null)
        {
            throw new InvalidOperationException($"NodeId {nodeId} was outside of TextNode range.");
        }

        var steps = SplitTextStep.GenerateSplitSteps(transaction, document, textNode, splitNode, offset);
        return (splitNode, steps);
    }
}