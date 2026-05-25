using DScratch.Nodes;

namespace DScratch.Transactions.Steps;

public class MoveRangeStep(DNode? start, DNode? end, DNode targetParent, DNode? targetOrigin) : IStep
{
    public IReadOnlyList<StepDiff?> Execute()
    {
        if (start is null && end is null) return [];
        
        var steps = new List<StepDiff?>();

        if (end is null)
        {
            start!.Origin = targetOrigin;
            
            var current = start;
            while (current is not null)
            {
                current.RightOrigin ??= targetOrigin?.RightOrigin;
                
                steps.Add(current.ToMovePrepStep());
                current.Parent = targetParent;
                steps.Add(current.ToMoveStep());
                current = current.RightOrigin;
            }
        }
        else if (start is null)
        {
            end.RightOrigin = targetOrigin?.RightOrigin;
            
            var current = end;
            while (current is not null)
            {
                current.Origin ??= targetOrigin;
                
                steps.Add(current.ToMovePrepStep());
                current.Parent = targetParent;
                steps.Add(current.ToMoveStep());
                current = current.Origin;
            }
        }
        else
        {
            start.Origin = targetOrigin;
            end.RightOrigin = targetOrigin?.RightOrigin;
            
            var current = start;
            while (current is not null && current.Id != end.Id)
            {
                steps.Add(current.ToMovePrepStep());
                current.Parent = targetParent;
                steps.Add(current.ToMoveStep());
                current = current.RightOrigin;
            }

            if (current is not null)
            {
                steps.Add(current.ToMovePrepStep());
                current.Parent = targetParent;
                steps.Add(current.ToMoveStep());
            }
        }
        
        return steps;
    }

    public IReadOnlyList<StepDiff> Revert(DScratchDocument document)
    {
        throw new NotImplementedException();
    }
}