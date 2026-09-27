using DScratch.Nodes;
using DScratch.Rendering;

namespace DScratch.Transactions.Steps;

public class MoveRangeStep(DNode? start, DNode? end, DNode targetParent, DNode? targetOrigin) : IStep
{
    public IReadOnlyList<StepDiff?> Execute(IRunningTransaction transaction, DScratchDocument document)
    {
        if (start is null && end is null) return [];

        var existingFirstChild = targetOrigin is null ? targetParent.FirstChild : null;
        
        if (end is null)
        {
            return MoveFrom(transaction, document, existingFirstChild);
        }
        
        if (start is null)
        {
            return MoveTo(transaction, document, existingFirstChild);
        }

        return start.Parent!.Id == end.Parent!.Id 
            ? MoveFromTo(transaction, document, existingFirstChild) 
            : throw new ArgumentException("Start and End node must be of the same parent.");
    }

    private IReadOnlyList<StepDiff?> MoveFromTo(IRunningTransaction transaction, DScratchDocument document, DNode? existingFirstChild)
    {
        var steps = new List<StepDiff?>();
        
        var previousOrigin = targetOrigin;
        var current = start;
        while (current is not null && current.Id != end!.Id)
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

    private List<StepDiff?> MoveTo(IRunningTransaction transaction, DScratchDocument document, DNode? existingFirstChild)
    {
        var steps = new List<StepDiff?>();
        
        var previousOrigin = targetOrigin?.NextSibling();
        var current = end;
        while (current is not null)
        {
            var origin = previousOrigin ?? existingFirstChild;

            if (!current.IsDeleted)
            {
                previousOrigin = MoveNode(
                    current: current, 
                    origin: origin,
                    rightOrigin: origin?.PreviousSibling(),
                    transaction: transaction, 
                    steps: steps,
                    document: document);
            }
            current = current.NextSibling();
        }
        
        return steps;
    }

    private List<StepDiff?> MoveFrom(IRunningTransaction transaction, DScratchDocument document, DNode? existingFirstChild)
    {
        var steps = new List<StepDiff?>();
        
        var previousOrigin = targetOrigin;
        var current = start;
        while (current is not null)
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