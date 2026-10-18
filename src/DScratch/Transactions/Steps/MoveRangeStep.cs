using DScratch.Nodes;
using DScratch.Rendering;

namespace DScratch.Transactions.Steps;

public class MoveRangeStep(NodeId? start, NodeId? end, DNode targetParent, NodeId? targetOriginId) : IStep
{
    public IReadOnlyList<StepDiff?> Execute(IRunningTransaction transaction, DScratchDocument document)
    {
        if (start is null && end is null) return [];
        
        var steps = new List<StepDiff?>();

        var existingFirstChild = targetOriginId is null ? targetParent.FirstChild : null;
        var targetOrigin = GetNodeOrDefault(targetOriginId);
        if (targetOrigin is TextNode targetOriginTextNode)
        {
            (targetOrigin, var stepDiffs) = GetLeftSplitPart(transaction, document, targetOriginTextNode, targetOriginId!.Value);
            steps.AddRange(stepDiffs);
        }
        
        var startNode = GetNodeOrDefault(start);
        if (startNode is TextNode startTextNode)
        {
            (startNode, var stepDiffs) = GetRightSplitPart(transaction, document, startTextNode, start!.Value);
            steps.AddRange(stepDiffs);
        }
        
        var endNode = GetNodeOrDefault(end);
        if (endNode is TextNode endTextNode)
        {
            (endNode, var stepDiffs) = GetRightSplitPart(transaction, document, endTextNode, end!.Value);
            steps.AddRange(stepDiffs);
        }

        startNode ??= endNode?.Parent?.FirstChild;
        
        if (startNode is null)
        {
            throw new ArgumentException($"Could not infer a start node for move step: Start: '{start}' - End: '{end}'");
        }
        
        if (endNode is not null && startNode.Parent!.Id != endNode.Parent!.Id)
        {
            throw new ArgumentException("Start and End node must be of the same parent.");
        }

        MoveNodes(
            startNode: startNode,
            endNode: endNode,
            targetOrigin: targetOrigin,
            transaction: transaction,
            steps: steps,
            existingFirstChild: existingFirstChild);

        return steps;
        
        DNode? GetNodeOrDefault(NodeId? nodeId)
        {
            return nodeId.HasValue
                ? document.FindNode(nodeId.Value)
                : null;
        }
    }

    private void MoveNodes(
        DNode? startNode,
        DNode? endNode,
        DNode? targetOrigin,
        IRunningTransaction transaction,
        List<StepDiff?> steps,
        DNode? existingFirstChild)
    {
        var current = startNode;
        while (current is not null && current.Id != endNode?.Id)
        {
            var next = current.NextSibling();
            if (!current.IsDeleted)
            {
                MoveNode(
                    current: current, 
                    origin: targetOrigin, 
                    rightOrigin: targetOrigin?.NextSibling() ?? existingFirstChild,
                    transaction: transaction, 
                    steps: steps);
                
                targetOrigin = current;
            }

            current = next;
        }
    }

    private void MoveNode(
        DNode current, 
        DNode? origin,
        DNode? rightOrigin,
        IRunningTransaction transaction,
        List<StepDiff?> steps)
    {
        current.Relink(origin?.LastId, rightOrigin?.Id);
        targetParent.InsertChild(current);
        transaction.NotifyNodeChange(current);
        steps.AddRange(current.ToMoveStep());
    }
    
    private static (DNode Node, IReadOnlyList<StepDiff?> StepDiffs) GetRightSplitPart(IRunningTransaction transaction, DScratchDocument document, TextNode textNode, NodeId nodeId)
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
    
    private static (DNode Node, IReadOnlyList<StepDiff?> StepDiffs) GetLeftSplitPart(IRunningTransaction transaction, DScratchDocument document, TextNode textNode, NodeId nodeId)
    {
        if (textNode.Length == 1 || textNode.LastId == nodeId)
        {
            return (textNode, []);
        }
        
        var offset = (int)(nodeId.Clock - textNode.Id.Clock + 1);
        var splitNode = textNode.Split(offset);

        if (splitNode is null)
        {
            throw new InvalidOperationException($"NodeId {nodeId} was outside of TextNode range.");
        }

        var steps = SplitTextStep.GenerateSplitSteps(transaction, document, textNode, splitNode, offset);
        return (textNode, steps);
    }

    public IReadOnlyList<StepDiff> Revert(DScratchDocument document)
    {
        throw new NotImplementedException();
    }
}