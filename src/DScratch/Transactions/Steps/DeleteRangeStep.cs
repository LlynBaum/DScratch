using DScratch.Nodes;
using DScratch.Rendering;

namespace DScratch.Transactions.Steps;

public class DeleteRangeStep(DNode? start, DNode? end) : IStep
{
    public IReadOnlyList<StepDiff?> Execute(IRunningTransaction transaction, DScratchDocument document)
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
                current = current.NextSibling();
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
                current = current.PreviousSibling();
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
                current = current.NextSibling();
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