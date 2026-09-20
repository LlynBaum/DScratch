using DScratch.Nodes;
using DScratch.Rendering;

namespace DScratch.Transactions.Steps;

public class MoveRangeStep(DNode? start, DNode? end, DNode targetParent, DNode? targetOrigin) : IStep
{
    public IReadOnlyList<StepDiff?> Execute(IRunningTransaction transaction, DScratchDocument document)
    {
        if (start is null && end is null) return [];
        
        var steps = new List<StepDiff?>();

        var existingFirstChild = targetOrigin is null ? targetParent.FirstChild : null;
        
        if (end is null)
        {
            var previousOrigin = targetOrigin;
            var current = start;
            while (current is not null)
            {
                var next = current.NextSibling();
                var origin = previousOrigin;
                if (!current.IsDeleted)
                {
                    current.Delete();
                    steps.Add(current.ToDeleteSteps());
                    transaction.NotifyNodeChange(current);

                    var newNode = transaction.NodeFactory.Recreate(
                        node: current, 
                        origin: origin, 
                        rightOrigin: origin?.NextSibling() ?? existingFirstChild);
                    
                    document.AddNode(newNode);
                    targetParent.InsertChild(newNode);
                    transaction.NotifyNodeChange(newNode);
                    steps.AddRange(current.ToInsertSteps());
                }
                
                previousOrigin = current;
                current = next;
            }
        }
        else if (start is null)
        {
            var previousRightOrigin = targetOrigin?.NextSibling();
            var current = end;
            while (current is not null)
            {
                var next = current.PreviousSibling();
                var rightOrigin = previousRightOrigin ?? existingFirstChild;

                if (!current.IsDeleted)
                {
                    current.Delete();
                    steps.Add(current.ToDeleteSteps());
                    transaction.NotifyNodeChange(current);
                    
                    var newNode = transaction.NodeFactory.Recreate(
                        node: current, 
                        origin: rightOrigin, 
                        rightOrigin: rightOrigin?.PreviousSibling());
                    
                    document.AddNode(newNode);
                    targetParent.InsertChild(newNode);
                    transaction.NotifyNodeChange(newNode);
                    steps.AddRange(current.ToInsertSteps());
                }

                previousRightOrigin = current;
                current = next;
            }
        }
        else
        {
            var previousOrigin = targetOrigin;
            var current = start;
            while (current is not null && current.Id != end.Id)
            {
                var next = current.NextSibling();
                var origin = previousOrigin;

                if (!current.IsDeleted)
                {
                    current.Delete();
                    steps.Add(current.ToDeleteSteps());
                    transaction.NotifyNodeChange(current);
                    
                    var newNode = transaction.NodeFactory.Recreate(
                        node: current, 
                        origin: origin, 
                        rightOrigin: origin?.NextSibling() ?? existingFirstChild);
                    
                    document.AddNode(newNode);
                    targetParent.InsertChild(newNode);
                    transaction.NotifyNodeChange(newNode);
                    steps.AddRange(current.ToInsertSteps());
                }
                
                previousOrigin = current;
                current = next;
            }

            if (current is not null)
            {
                if (!current.IsDeleted)
                {
                    current.Delete();
                    steps.Add(current.ToDeleteSteps());
                    transaction.NotifyNodeChange(current);
                    
                    var newNode = transaction.NodeFactory.Recreate(
                        node: current, 
                        origin: previousOrigin, 
                        rightOrigin: previousOrigin?.NextSibling() ?? existingFirstChild);
                    
                    document.AddNode(newNode);
                    targetParent.InsertChild(newNode);
                    transaction.NotifyNodeChange(newNode);
                    steps.AddRange(current.ToInsertSteps());
                }
            }
        }
        
        return steps;
    }

    public IReadOnlyList<StepDiff> Revert(DScratchDocument document)
    {
        throw new NotImplementedException();
    }
}