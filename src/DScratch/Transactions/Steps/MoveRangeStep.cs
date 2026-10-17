using DScratch.Nodes;
using DScratch.Rendering;

namespace DScratch.Transactions.Steps;

public class MoveRangeStep(NodeId? start, NodeId? end, DNode targetParent, NodeId? targetOriginId) : IStep
{
    public IReadOnlyList<StepDiff?> Execute(IRunningTransaction transaction, DScratchDocument document)
    {
        if (start is null && end is null) return [];

        var existingFirstChild = targetOriginId is null ? targetParent.FirstChild : null;
        var targetOrigin = GetNodeOrDefault(targetOriginId);
        
        var startNode = GetNodeOrDefault(start);
        var endNode = GetNodeOrDefault(end) ?? startNode?.Parent?.LastChild;
        startNode ??= endNode?.Parent?.FirstChild;

        if (startNode is null)
        {
            throw new ArgumentException($"Could not infer a start node for move step: Start: '{start}' - End: '{end}'");
        }

        if (endNode is null)
        {
            throw new ArgumentException($"Could not infer a end node for move step: Start: '{start}' - End: '{end}'");
        }

        if (startNode.Parent!.Id != endNode.Parent!.Id)
        {
            throw new ArgumentException("Start and End node must be of the same parent.");
        }

        return MoveNodes(
            startNode: startNode,
            endNode: endNode,
            targetOrigin: targetOrigin,
            transaction: transaction,
            existingFirstChild: existingFirstChild);
        
        DNode? GetNodeOrDefault(NodeId? nodeId)
        {
            return nodeId.HasValue
                ? document.FindNode(nodeId.Value)
                : null;
        }
    }

    private List<StepDiff?> MoveNodes(
        DNode? startNode,
        DNode? endNode,
        DNode? targetOrigin,
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
                
                targetOrigin = current;
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

    public IReadOnlyList<StepDiff> Revert(DScratchDocument document)
    {
        throw new NotImplementedException();
    }
}