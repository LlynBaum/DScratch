using DScratch.Nodes;
using DScratch.Rendering;

namespace DScratch.Transactions.Steps;

public class MoveRangeStep(DNode? start, DNode? end, DNode targetParent, DNode? targetOrigin) : IStep
{
    public IReadOnlyList<StepDiff?> Execute(IRunningTransaction transaction, DScratchDocument document) // TODO: work like DeleteRangeStep
    {
        if (start is null && end is null) return [];

        var existingFirstChild = targetOrigin is null ? targetParent.FirstChild : null;
        var startNode = start;
        var endNode = end;
        
        if (end is null)
        {
            endNode = startNode!.Parent!.LastChild;
        }
        
        if (start is null)
        {
            startNode = end!.Parent!.FirstChild;
        }

        return startNode!.Parent!.Id != endNode!.Parent!.Id 
            ? throw new ArgumentException("Start and End node must be of the same parent.") 
            : MoveNodes(
                startNode: startNode, 
                endNode: endNode,
                transaction: transaction,
                existingFirstChild: existingFirstChild);
    }

    private List<StepDiff?> MoveNodes(
        DNode? startNode,
        DNode? endNode,
        IRunningTransaction transaction,
        DNode? existingFirstChild)
    {
        var steps = new List<StepDiff?>();

        var current = startNode;
        while (current is not null && current.Id != endNode!.Id)
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
            }
            current = next;
        }

        if (current is not null && !current.IsDeleted)
        {
            MoveNode(
                current: current, 
                origin: targetOrigin, 
                rightOrigin: targetOrigin?.NextSibling() ?? existingFirstChild,
                transaction: transaction, 
                steps: steps);
        }

        return steps;
    }

    private void MoveNode(
        DNode current, 
        DNode? origin,
        DNode? rightOrigin,
        IRunningTransaction transaction,
        List<StepDiff?> steps)
    {
        current.Relink(origin?.Id, rightOrigin?.Id);
        targetParent.InsertChild(current);
        transaction.NotifyNodeChange(current);
        steps.AddRange(current.ToMoveStep());
    }
    
    private static (DNode Node, IReadOnlyList<StepDiff?> StepDiffs) SplitStartTextNode(IRunningTransaction transaction, DScratchDocument document, TextNode textNode, NodeId nodeId)
    {
        if (textNode.Length == 1) // TODO: test scenario
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
        return (splitNode, steps); // TODO: test scenario
    }
    
    private static (DNode Node, IReadOnlyList<StepDiff?> StepDiffs) SplitEndTextNode(IRunningTransaction transaction, DScratchDocument document, TextNode textNode, NodeId nodeId)
    {
        if (textNode.Length == 1 || textNode.LastId == nodeId) // TODO: test scenario
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
        return (textNode, steps); // TODO: test scenario
    }

    public IReadOnlyList<StepDiff> Revert(DScratchDocument document)
    {
        throw new NotImplementedException();
    }
}