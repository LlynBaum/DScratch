using DScratch.Nodes;

namespace DScratch.Transactions.Steps;

public class MoveRangeStep(DNode? start, DNode? end, DNode targetParent, DNode? targetOrigin) : IStep
{
    public IReadOnlyList<StepDiff?> Execute()
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
                var next = current.RightOrigin;
                var origin = previousOrigin;
                if (!current.IsDeleted)
                {
                    steps.AddRange(current.ToMoveStep(node =>
                    {
                        node.Remove();
                        node.Origin = origin;
                        node.RightOrigin = origin?.RightOrigin ?? existingFirstChild;
                        targetParent.InsertChild(node);
                    }));
                }
                else
                {
                    current.Remove();
                    current.Origin = origin;
                    current.RightOrigin = origin?.RightOrigin ?? existingFirstChild;
                    targetParent.InsertChild(current);
                }
                
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
                var rightOrigin = previousRightOrigin ?? existingFirstChild;

                if (!current.IsDeleted)
                {
                    steps.AddRange(current.ToMoveStep(node =>
                    {
                        node.Remove();
                        node.RightOrigin = rightOrigin;
                        node.Origin = rightOrigin?.Origin;
                        targetParent.InsertChild(node);
                    }));
                }
                else
                {
                    current.Remove();
                    current.RightOrigin = rightOrigin;
                    current.Origin = rightOrigin?.Origin;
                    targetParent.InsertChild(current);
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
                var next = current.RightOrigin;
                var origin = previousOrigin;

                if (!current.IsDeleted)
                {
                    steps.AddRange(current.ToMoveStep(node =>
                    {
                        node.Remove();
                        node.Origin = origin;
                        node.RightOrigin = origin?.RightOrigin ?? existingFirstChild;
                        targetParent.InsertChild(node);
                    }));
                }
                else
                {
                    current.Remove();
                    current.Origin = origin;
                    current.RightOrigin = origin?.RightOrigin ?? existingFirstChild;
                    targetParent.InsertChild(current);
                }
                
                previousOrigin = current;
                current = next;
            }

            if (current is not null)
            {
                if (!current.IsDeleted)
                {
                    steps.AddRange(current.ToMoveStep(node =>
                    {
                        node.Remove();
                        node.Origin = previousOrigin;
                        node.RightOrigin = previousOrigin?.RightOrigin ?? existingFirstChild;
                        targetParent.InsertChild(node);
                    }));
                }
                else
                {
                    current.Remove();
                    current.Origin = previousOrigin;
                    current.RightOrigin = previousOrigin?.RightOrigin ?? existingFirstChild;
                    targetParent.InsertChild(current);
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