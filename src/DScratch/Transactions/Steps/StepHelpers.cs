using DScratch.Nodes;
using DScratch.Nodes.NodeTypes;

namespace DScratch.Transactions.Steps;

internal static class StepHelpers
{
    extension(DNode node)
    {
        public StepDiff[] ToInsertSteps()
        {
            var parentId = node.ParentElement?.Id ?? NodeId.Root;
            
            return node switch
            {
                RootNode => [..node.ActiveChildNodes.SelectMany(c => c.ToInsertSteps())],
                TextNode textNode => InsertTextNode(textNode, parentId),
                IElement => 
                [
                    new StepDiff.InsertElementDiff(parentId.Value, node.GetFirstActiveOrigin()?.Id.Value, node.TagName, node.Id.Value),
                    ..node.ActiveChildNodes.SelectMany(c => c.ToInsertSteps())
                ],
                _ => throw new ArgumentException("Node type is not an element, text or char node.")
            };
        }

        public StepDiff? ToDeleteSteps()
        {
            return node switch
            {
                RootNode => null,
                _ => new StepDiff.DeleteElementDiff(node.Id.Value)
            };
        }

        public StepDiff? ToMoveStep()
        {
            return node switch
            {
                RootNode => null,
                _ => new StepDiff.MoveDiff(
                    TargetNodeId: node.Id.Value,
                    TargetParentId: node.ParentElement!.Id.Value,
                    PreviousSiblingId: node.GetFirstActiveOrigin()?.Id.Value)
            };
        }
    }
    
    private static StepDiff[] InsertTextNode(TextNode textNode, NodeId parentId)
    {
        return [
            new StepDiff.InsertElementDiff(
                ParentId: parentId.Value,
                PreviousSiblingId: textNode.Origin?.Id.Value,
                TagName: textNode.TagName,
                NewNodeId: textNode.Id.Value),
            new StepDiff.InsertTextDiff(
                ParentId: textNode.Id.Value,
                Offset: 0,
                Text: textNode.TextContent)
        ];
    }
}
