namespace DScratch.Transactions.Steps;

public class DeleteStep(NodePath path, int offset) : IStep
{
    public IStep.StepDiff Execute(DScratchDocument document)
    {
        var parent = document.FindNode(path);
        
        if (parent is null)
        {
            throw new ArgumentException("Could not find parent Node at the expected path.");
        }

        var nodeToDelete = parent.GetChild(offset - 1);
        if (nodeToDelete is null)
        {
            throw new ArgumentException("Can not find node to delete.");
        }
        
        parent.DeleteChild(nodeToDelete.Id);
        
        return new DeleteDiff(path, offset);
    }

    public IStep.StepDiff Revert(DScratchDocument document)
    {
        throw new NotImplementedException();
    }
    
    public record DeleteDiff(NodePath Path, int Offset) : IStep.StepDiff(IStep.StepType.Delete);
}