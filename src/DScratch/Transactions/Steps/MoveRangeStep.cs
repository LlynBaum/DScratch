using DScratch.Nodes;

namespace DScratch.Transactions.Steps;

public class MoveRangeStep(DNode? start, DNode? end, DNode targetParent, DNode? targetOrigin) : IStep
{
    public void Execute(IRunningTransaction transaction, DScratchDocument document)
    {
        if (start is null && end is null) return;

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
                    current.Remove();
                    current.Origin = origin;
                    current.RightOrigin = origin?.RightOrigin ?? existingFirstChild;
                    targetParent.InsertChild(current);
                    transaction.NotifyNodeChange(new ModifiedNode(current, Modification.Delete));
                    transaction.NotifyNodeChange(new ModifiedNode(current.Parent!, Modification.Changed));
                }
                else
                {
                    current.Remove();
                    current.Origin = origin;
                    current.RightOrigin = origin?.RightOrigin ?? existingFirstChild;
                    targetParent.InsertChild(current);
                    transaction.NotifyNodeChange(new ModifiedNode(current, Modification.Delete));
                    transaction.NotifyNodeChange(new ModifiedNode(current.Parent!, Modification.Changed));
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
                    current.Remove();
                    current.RightOrigin = rightOrigin;
                    current.Origin = rightOrigin?.Origin;
                    targetParent.InsertChild(current);
                    transaction.NotifyNodeChange(new ModifiedNode(current, Modification.Delete));
                    transaction.NotifyNodeChange(new ModifiedNode(current.Parent!, Modification.Changed));
                }
                else
                {
                    current.Remove();
                    current.RightOrigin = rightOrigin;
                    current.Origin = rightOrigin?.Origin;
                    targetParent.InsertChild(current);
                    transaction.NotifyNodeChange(new ModifiedNode(current, Modification.Delete));
                    transaction.NotifyNodeChange(new ModifiedNode(current.Parent!, Modification.Changed));
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
                    current.Remove();
                    current.Origin = origin;
                    current.RightOrigin = origin?.RightOrigin ?? existingFirstChild;
                    targetParent.InsertChild(current);
                    transaction.NotifyNodeChange(new ModifiedNode(current, Modification.Delete));
                    transaction.NotifyNodeChange(new ModifiedNode(current.Parent!, Modification.Changed));
                }
                else
                {
                    current.Remove();
                    current.Origin = origin;
                    current.RightOrigin = origin?.RightOrigin ?? existingFirstChild;
                    targetParent.InsertChild(current);
                    transaction.NotifyNodeChange(new ModifiedNode(current, Modification.Delete));
                    transaction.NotifyNodeChange(new ModifiedNode(current.Parent!, Modification.Changed));
                }
                
                previousOrigin = current;
                current = next;
            }

            if (current is not null)
            {
                if (!current.IsDeleted)
                {
                    current.Remove();
                    current.Origin = previousOrigin;
                    current.RightOrigin = previousOrigin?.RightOrigin ?? existingFirstChild;
                    targetParent.InsertChild(current);
                    transaction.NotifyNodeChange(new ModifiedNode(current, Modification.Delete));
                    transaction.NotifyNodeChange(new ModifiedNode(current.Parent!, Modification.Changed));
                }
                else
                {
                    current.Remove();
                    current.Origin = previousOrigin;
                    current.RightOrigin = previousOrigin?.RightOrigin ?? existingFirstChild;
                    targetParent.InsertChild(current);
                    transaction.NotifyNodeChange(new ModifiedNode(current, Modification.Delete));
                    transaction.NotifyNodeChange(new ModifiedNode(current.Parent!, Modification.Changed));
                }
            }
        }
    }

    public void Revert(DScratchDocument document)
    {
        throw new NotImplementedException();
    }
}