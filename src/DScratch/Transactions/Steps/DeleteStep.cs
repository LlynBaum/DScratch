using DScratch.Nodes;

namespace DScratch.Transactions.Steps;

public class DeleteStep(DNode node) : IStep
{
    public void Execute(IRunningTransaction transaction, DScratchDocument document)
    {
        node.Delete();
        transaction.NotifyNodeChange(node);
    }

    public void Revert(DScratchDocument document)
    {
        throw new NotImplementedException();
    }
}