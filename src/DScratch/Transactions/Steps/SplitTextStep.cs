using DScratch.Nodes;
using DScratch.Rendering;

namespace DScratch.Transactions.Steps;

public class SplitTextStep(TextNode node, TextNode splitNode, int offset) : IStep
{
    public IReadOnlyList<StepDiff?> Execute(IRunningTransaction transaction, DScratchDocument document) => GenerateSplitSteps(transaction, document, node, splitNode, offset);
    
    internal static IReadOnlyList<StepDiff?> GenerateSplitSteps(
        IRunningTransaction transaction,
        DScratchDocument document,
        TextNode node,
        TextNode splitNode,
        int offset)
    {
        document.AddNode(splitNode);
        transaction.NotifyNodeChange(node);
        transaction.NotifyNodeChange(splitNode);
        
        return
        [
            new StepDiff.DeleteTextDiff(node.Id.Value, offset, splitNode.Length),
            ..splitNode.ToInsertSteps()
        ];
    }

    public IReadOnlyList<StepDiff?> Revert(DScratchDocument document)
    {
        throw new NotImplementedException();
    }
}