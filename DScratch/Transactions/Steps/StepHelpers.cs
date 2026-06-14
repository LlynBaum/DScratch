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
                RootNode => [..node.ChildNodes.SelectMany(c => c.ToInsertSteps())],
                TextNode textNode => InsertTextNode(textNode, parentId),
                IElement => 
                [
                    new StepDiff.InsertElementDiff(parentId.Value, node.GetFirstActiveOrigin()?.Id.Value, node.TagName, node.Id.Value),
                    ..node.ChildNodes.SelectMany(c => c.ToInsertSteps())
                ],
                _ => throw new ArgumentException("Node type is not an element, text or char node.")
            };
        }

        public StepDiff? ToDeleteSteps()
        {
            return node switch
            {
                RootNode => null,
                TextNode textNode => new StepDiff.DeleteTextDiff(node.ParentElement!.Id.Value, node.ParentElement?.FindAbsolutTextOffset(textNode) ?? 0, textNode.Length),
                IElement => new StepDiff.DeleteElementDiff(node.Id.Value),
                _ => throw new ArgumentException("Node type is not an element, text or char.")
            };
        }

        public StepDiff?[] ToMoveStep(Action<DNode> move)
        {
            var result = new List<StepDiff?>
            {
                node.ToMovePrepStep()
            };
            
            move.Invoke(node);
            result.AddRange(node.ToFinalizedMoveStep());
            return [..result];
        }

        private StepDiff.DeleteTextDiff? ToMovePrepStep()
        {
            return node switch
            {
                TextNode textNode => new StepDiff.DeleteTextDiff(
                    ParentId: node.ParentElement!.Id.Value, 
                    Offset: node.ParentElement?.FindAbsolutTextOffset(textNode) ?? 0, 
                    Length: textNode.Length),
                _ => null
            };
        }
        
        private StepDiff[] ToFinalizedMoveStep()
        {
            return node switch
            {
                RootNode => [],
                TextNode textNode => InsertTextNode(textNode, node.ParentElement!.Id),
                IElement =>
                [
                    new StepDiff.MoveDiff(
                        TargetNodeId: node.Id.Value,
                        TargetParentId: node.ParentElement!.Id.Value,
                        PreviousSiblingId: node.GetFirstActiveOrigin()?.Id.Value)
                ],
                _ => throw new ArgumentException("Node type is not an element.")
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
