using DScratch.Nodes;

namespace DScratch.Transactions.Steps;

public class DeleteRangeStep(DNode? start, DNode? end) : IStep
{
    public void Execute(IRunningTransaction transaction, DScratchDocument document)
    {
        if (start is null && end is null) return;

        if (end is null)
        {
            var current = start;
            while (current is not null)
            {
                current.Delete();
                transaction.NotifyNodeChange(new ModifiedNode(current, Modification.Delete));
                current = current.RightOrigin;
            }
        }
        else if (start is null)
        {
            var current = end;
            while (current is not null)
            {
                current.Delete();
                transaction.NotifyNodeChange(new ModifiedNode(current, Modification.Delete));
                current = current.Origin;
            }
        }
        else
        {
            var current = start;
            while (current is not null && current.Id != end.Id)
            {
                current.Delete();
                transaction.NotifyNodeChange(new ModifiedNode(current, Modification.Delete));
                current = current.RightOrigin;
            }

            if (current is not null)
            {
                current.Delete();
                transaction.NotifyNodeChange(new ModifiedNode(current, Modification.Delete));
            }
        }
    }

    public void Revert(DScratchDocument document)
    {
        throw new NotImplementedException();
    }
}