using DScratch.Nodes;
using DScratch.Rendering;

namespace DScratch.Transactions.Steps;

public class MoveRangeStep(DNode? start, DNode? end, DNode targetParent, DNode? targetOrigin) : IStep
{
    public IReadOnlyList<StepDiff?> Execute(IRunningTransaction transaction, DScratchDocument document)
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
                document: document, 
                existingFirstChild: existingFirstChild);
    }

    private List<StepDiff?> MoveNodes(
        DNode? startNode,
        DNode? endNode,
        IRunningTransaction transaction,
        DScratchDocument document, 
        DNode? existingFirstChild)
    {
        var steps = new List<StepDiff?>();
        
        var previousOrigin = targetOrigin;
        var current = startNode;
        while (current is not null && current.Id != endNode!.Id)
        {
            if (!current.IsDeleted)
            {
                previousOrigin = MoveNode(
                    current: current, 
                    origin: previousOrigin, 
                    rightOrigin: previousOrigin?.NextSibling() ?? existingFirstChild,
                    transaction: transaction, 
                    steps: steps,
                    document: document);
            }
            current = current.NextSibling();
        }

        if (current is not null && !current.IsDeleted)
        {
            MoveNode(
                current: current, 
                origin: previousOrigin, 
                rightOrigin: previousOrigin?.NextSibling() ?? existingFirstChild,
                transaction: transaction, 
                steps: steps,
                document: document);
        }

        return steps;
    }

    private DNode MoveNode(
        DNode current, 
        DNode? origin,
        DNode? rightOrigin,
        IRunningTransaction transaction,
        List<StepDiff?> steps, 
        DScratchDocument document)
    {
        current.Delete();
        steps.Add(current.ToDeleteSteps());
        transaction.NotifyNodeChange(current);

        var newNode = transaction.NodeFactory.Recreate(
            node: current, 
            origin: origin,
            rightOrigin: rightOrigin);
        
        document.AddNode(newNode);
        targetParent.InsertChild(newNode);
        transaction.NotifyNodeChange(newNode);
        steps.AddRange(current.ToInsertSteps());
        
        return newNode;
    }

    public IReadOnlyList<StepDiff> Revert(DScratchDocument document)
    {
        throw new NotImplementedException();
    }
}