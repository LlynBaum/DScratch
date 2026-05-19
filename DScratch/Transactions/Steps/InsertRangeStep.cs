using DScratch.Nodes;

namespace DScratch.Transactions.Steps;

internal class InsertRangeStep(DNode first, DCharNode last, NodePath path, int offset) : IStep
{
    public IReadOnlyList<StepDiff> Execute(DScratchDocument document)
    {
        var parent = document.FindNode(path);
        if (parent is null)
        {
            throw new ArgumentException("Could not find parent Node at the expected path.");
        }
        
        var origin = parent.GetChild(offset - 1);
        var rightOrigin = parent.GetChild(offset);

        var steps = new List<StepDiff>();
        var current = first;
        while (current is not null)
        {
            steps.Add(current.ToInsert(path, offset));
            current.Parent = parent;
            current = current.RightOrigin;
        }
        
        first.Origin = origin;
        last.RightOrigin = rightOrigin;
        
        parent.InsertChildRange(first, last);
        return steps;
    }

    public IReadOnlyList<StepDiff> Revert(DScratchDocument document)
    {
        throw new NotImplementedException();
    }
}