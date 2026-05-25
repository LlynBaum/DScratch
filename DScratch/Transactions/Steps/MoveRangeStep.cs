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
                steps.AddRange(current.ToMoveStep(node =>
                {
                    node.RightOrigin ??= targetOrigin?.RightOrigin;
                    targetParent.InsertChild(node);
                }));
                current = current.RightOrigin;
            }
        }
        else if (start is null)
        {
            end.RightOrigin = targetOrigin?.RightOrigin;
            
            var current = end;
            while (current is not null)
            {
                steps.AddRange(current.ToMoveStep(node =>
                {
                    node.Origin ??= targetOrigin;
                    targetParent.InsertChild(node);
                }));
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
                steps.AddRange(current.ToMoveStep(targetParent.InsertChild));
                current = current.RightOrigin;
            }

            if (current is not null)
            {
                steps.AddRange(current.ToMoveStep(targetParent.InsertChild));
            }
        }
        
        return steps;
    }

    public IReadOnlyList<StepDiff> Revert(DScratchDocument document)
    {
        throw new NotImplementedException();
    }
}