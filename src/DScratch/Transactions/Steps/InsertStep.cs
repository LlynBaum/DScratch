using DScratch.Nodes;

namespace DScratch.Transactions.Steps;

internal class InsertStep(DNode node, DNode parent) : IStep
{
    public void Execute(IRunningTransaction transaction, DScratchDocument document)
    {
        parent.InsertChild(node);
        transaction.NotifyNodeChange(new ModifiedNode(node, Modification.Insert));
    }

    public void Revert(DScratchDocument document)
    {
        throw new NotImplementedException();
    }
}