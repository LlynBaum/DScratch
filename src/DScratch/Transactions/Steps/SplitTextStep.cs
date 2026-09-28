using DScratch.Nodes;
using DScratch.Rendering;

namespace DScratch.Transactions.Steps;

public class SplitTextStep(TextNode node, TextNode splitNode, int offset) : IStep
{
    public IReadOnlyList<StepDiff?> Execute(IRunningTransaction _, DScratchDocument __) => GenerateSplitSteps(node, splitNode, offset);
    
    internal static IReadOnlyList<StepDiff?> GenerateSplitSteps(TextNode node, TextNode splitNode, int offset)
    {
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