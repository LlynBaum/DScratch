using DScratch.Nodes;

namespace DScratch.Transactions.Steps;

internal class InsertStep(DNode node, DNode parent) : IStep
{
    public IReadOnlyList<StepDiff?> Execute()
    {
        if (node is TextNode { Origin: TextNode origin } textNode && textNode.Origin.Id.IsContinuesTo(textNode.Id))
        { 
            origin.AddText(textNode.TextContent);
        }
        else
        {
            parent.InsertChild(node);
        }
        
        return node.ToInsertSteps();
    }

    public IReadOnlyList<StepDiff> Revert(DScratchDocument document)
    {
        throw new NotImplementedException();
    }
}