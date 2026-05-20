namespace DScratch.Transactions.Steps;

public class DeleteStep(NodePath path, int offset) : IStep
{
    public IReadOnlyList<StepDiff> Execute(DScratchDocument document)
    {
        var parent = document.FindNode(path);
        
        if (parent is null)
        {
            throw new ArgumentException("Could not find parent Node at the expected path.");
        }

        var nodeToDelete = parent.ChildNodes[offset - 1];
        if (nodeToDelete is null)
        {
            throw new ArgumentException("Can not find node to delete.");
        }
        
        nodeToDelete.Delete();
        return [nodeToDelete.ToDelete(path, offset)];
    }

    public IReadOnlyList<StepDiff> Revert(DScratchDocument document)
    {
        throw new NotImplementedException();
    }
}