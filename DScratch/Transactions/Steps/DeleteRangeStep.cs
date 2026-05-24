using DScratch.Nodes;

namespace DScratch.Transactions.Steps;

public class DeleteRangeStep(DNode? start, DNode? end) : IStep
{
    public IReadOnlyList<StepDiff> Execute()
    {
        if (start is null && end is null) return [];

        var steps = new List<StepDiff>();
        
        if (end is null)
        {
            var current = start;
            while (current is not null)
            {
                steps.Add(current.ToDeleteSteps());
                current.Delete();
                current = current.RightOrigin;
            }
        }
        else if (start is null)
        {
            var current = end;
            while (current is not null)
            {
                steps.Add(current.ToDeleteSteps());
                current.Delete();
                current = current.Origin;
            }
        }
        else
        {
            var current = start;
            while (current is not null && current.Id != end.Id)
            {
                steps.Add(current.ToDeleteSteps());
                current.Delete();
                current = current.RightOrigin;
            }

            if (current is not null)
            {
                steps.Add(current.ToDeleteSteps());
                current.Delete();
            }
        }

        return steps;
    }

    public IReadOnlyList<StepDiff> Revert(DScratchDocument document)
    {
        throw new NotImplementedException();
    }
}