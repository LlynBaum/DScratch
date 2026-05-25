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
            var previousOrigin = targetOrigin;
            var current = start;
            while (current is not null)
            {
                var next = current.RightOrigin;
                var origin = previousOrigin;
                steps.AddRange(current.ToMoveStep(node =>
                {
                    node.Remove();
                    node.Origin = origin;
                    node.RightOrigin = origin?.RightOrigin ?? targetParent.FirstChild;
                    targetParent.InsertChild(node);
                }));
                
                previousOrigin = current;
                current = next;
            }
        }
        else if (start is null)
        {
            var previousRightOrigin = targetOrigin?.RightOrigin;
            var current = end;
            while (current is not null)
            {
                var next = current.Origin;
                var rightOrigin = previousRightOrigin ?? targetParent.FirstChild;
                steps.AddRange(current.ToMoveStep(node =>
                {
                    node.Remove();
                    node.RightOrigin = rightOrigin;
                    node.Origin = rightOrigin?.Origin;
                    targetParent.InsertChild(node);
                }));
                
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
                var next = current.RightOrigin;
                var origin = previousOrigin;
                steps.AddRange(current.ToMoveStep(node =>
                {
                    node.Remove();
                    node.Origin = origin;
                    node.RightOrigin = origin?.RightOrigin ?? targetParent.FirstChild;
                    targetParent.InsertChild(node);
                }));
                
                previousOrigin = current;
                current = next;
            }

            if (current is not null)
            {
                steps.AddRange(current.ToMoveStep(node =>
                {
                    node.Remove();
                    node.Origin = previousOrigin;
                    node.RightOrigin = previousOrigin?.RightOrigin ?? targetParent.FirstChild;
                    targetParent.InsertChild(node);
                }));
            }
        }
        
        return steps;
    }

    public IReadOnlyList<StepDiff> Revert(DScratchDocument document)
    {
        throw new NotImplementedException();
    }
}