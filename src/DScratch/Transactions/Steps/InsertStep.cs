using DScratch.Nodes;
using DScratch.Rendering;

namespace DScratch.Transactions.Steps;

internal class InsertStep(DNode node, DNode parent) : IStep
{
    public IReadOnlyList<StepDiff?> Execute(IRunningTransaction transaction, DScratchDocument document)
    {
        var splitSteps = EnsureOriginExists(transaction, document);
        parent.InsertChild(node);
        transaction.NotifyNodeChange(node);
        document.AddNode(node);
        return [..splitSteps, ..node.ToInsertSteps()];
    }

    private IReadOnlyList<StepDiff?> EnsureOriginExists(IRunningTransaction transaction, DScratchDocument document)
    {
        if (!node.Origin.HasValue && !node.RightOrigin.HasValue)
        {
            return [];
        }

        List<StepDiff?> stepDiffs = [];

        if (node.Origin.HasValue)
        {
            var originNode = document.FindNode(node.Origin.Value);
            if (originNode.Parent != parent)
            {
                throw new InvalidOperationException($"Node {originNode.Id} has Parent {originNode.Parent?.Id} but expected {parent.Id}");
            }
            
            if (originNode.LastId != node.Origin && originNode is TextNode textNode)
            {
                var offset = (int)(node.Origin.Value.Clock - textNode.Id.Clock + 1);
                var splitNode = textNode.Split(offset);
                if (splitNode is not null)
                {
                    stepDiffs.AddRange(SplitTextStep.GenerateSplitSteps(transaction, document, textNode, splitNode, offset));
                }
            }
        }
        
        if (node.RightOrigin.HasValue)
        {
            var rightOriginNode = document.FindNode(node.RightOrigin.Value);
            if (rightOriginNode.Parent != parent)
            {
                throw new InvalidOperationException($"Node {rightOriginNode.Id} has Parent {rightOriginNode.Parent?.Id} but expected {parent.Id}");
            }
            
            if (rightOriginNode.Id != node.RightOrigin && rightOriginNode is TextNode textNode)
            {
                var offset = (int)(node.RightOrigin.Value.Clock - textNode.Id.Clock);
                var splitNode = textNode.Split(offset);
                if (splitNode is not null)
                {
                    stepDiffs.AddRange(SplitTextStep.GenerateSplitSteps(transaction, document, textNode, splitNode, offset));
                }
            }
        }
        
        return stepDiffs;
    }

    public IReadOnlyList<StepDiff> Revert(DScratchDocument document)
    {
        throw new NotImplementedException();
    }
}