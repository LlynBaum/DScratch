using DScratch.Nodes;

namespace DScratch.Transactions.Steps;

public class DeleteRangeStep(DNode? start, DNode? end) : IStep
{
    public IReadOnlyList<StepDiff?> Execute(IRunningTransaction transaction)
    {
        if (start is null && end is null) return [];

        var steps = new List<StepDiff?>();
        
        if (end is null)
        {
            var current = start;
            while (current is not null)
            {
                current.Delete();
                transaction.NotifyNodeChange(current);
                steps.Add(current.ToDeleteSteps());
                current = current.RightOrigin;
            }
        }
        else if (start is null)
        {
            var current = end;
            while (current is not null)
            {
                current.Delete();
                transaction.NotifyNodeChange(current);
                steps.Add(current.ToDeleteSteps());
                current = current.Origin;
            }
        }
        else
        {
            var current = start;
            while (current is not null && current.Id != end.Id)
            {
                current.Delete();
                transaction.NotifyNodeChange(current);
                steps.Add(current.ToDeleteSteps());
                current = current.RightOrigin;
            }

            if (current is not null)
            {
                current.Delete();
                transaction.NotifyNodeChange(current);
                steps.Add(current.ToDeleteSteps());
            }
        }

        return steps;
    }

    public IReadOnlyList<StepDiff> Revert(DScratchDocument document)
    {
        throw new NotImplementedException();
    }
}