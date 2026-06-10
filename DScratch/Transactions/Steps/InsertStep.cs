using DScratch.Nodes;

namespace DScratch.Transactions.Steps;

internal class InsertStep(DNode node, DNode parent) : IStep
{
    public IReadOnlyList<StepDiff?> Execute(IRunningTransaction transaction)
    {
        if (node is TextNode { Origin: TextNode origin } textNode && origin.LastId.IsContinuesTo(textNode.Id))
        { 
            origin.AddText(textNode.TextContent);
        }
        else
        {
            parent.InsertChild(node);
        }
        
        transaction.NotifyNodeChange(node);
        return node.ToInsertSteps();
    }

    public IReadOnlyList<StepDiff> Revert(DScratchDocument document)
    {
        throw new NotImplementedException();
    }
}