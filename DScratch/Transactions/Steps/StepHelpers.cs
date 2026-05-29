using DScratch.Nodes;
using DScratch.Nodes.NodeTypes;

namespace DScratch.Transactions.Steps;

internal static class StepHelpers
{
    extension(DNode node)
    {
        public StepDiff[] ToInsertSteps()
        {
            // We can not start the path at this node, since it is not in the DOM. So we take the parent node as first possible element
            var parentId = node.ParentElement?.Id ?? NodePath.RootId;
            
            return node switch
            {
                RootNode => [..node.ChildNodes.SelectMany(c => c.ToInsertSteps())],
                CharNode charNode => [new StepDiff.InsertTextDiff(parentId, node.ParentElement?.FindAbsolutTextOffset(charNode) ?? 0, charNode.Value.ToString())],
                TextNode textNode => [new StepDiff.InsertTextDiff(parentId, node.ParentElement?.FindAbsolutTextOffset(textNode) ?? 0, textNode.TextContent)],
                IInlineElement element => 
                [
                    new StepDiff.InsertElementInlineDiff(parentId, node.ParentElement?.FindAbsolutTextOffset<IInlineElement>(node) ?? 0, element.TagName, node.Id),
                    ..node.ChildNodes.SelectMany(c => c.ToInsertSteps())
                ],
                IBlockElement element => 
                [
                    new StepDiff.InsertElementBlockDiff(parentId, node.OriginElement?.Id, element.TagName, node.Id),
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
                CharNode charNode => new StepDiff.DeleteTextDiff(node.ParentElement!.Id, node.ParentElement?.FindAbsolutTextOffset(charNode) ?? 0, 1),
                TextNode textNode => new StepDiff.DeleteTextDiff(node.ParentElement!.Id, node.ParentElement?.FindAbsolutTextOffset(textNode) ?? 0, textNode.Length),
                IElement => new StepDiff.DeleteElementDiff(node.Id),
                _ => throw new ArgumentException("Node type is not an element, text or char.")
            };
        }

        public StepDiff?[] ToMoveStep(Action<DNode> move)
        {
            var result = new StepDiff?[2];
            result[0] = node.ToMovePrepStep();
            move.Invoke(node);
            result[1] = node.ToFinalizedMoveStep();
            return result;
        }

        private StepDiff.DeleteTextDiff? ToMovePrepStep()
        {
            return node switch
            {
                RootNode => null,
                CharNode charNode => new StepDiff.DeleteTextDiff(
                    ParentId: node.ParentElement!.Id, 
                    Offset: node.ParentElement?.FindAbsolutTextOffset(charNode) ?? 0, 
                    Length: 1),
                TextNode textNode => new StepDiff.DeleteTextDiff(
                    ParentId: node.ParentElement!.Id, 
                    Offset: node.ParentElement?.FindAbsolutTextOffset(textNode) ?? 0, 
                    Length: textNode.Length),
                _ => null
            };
        }
        
        private StepDiff? ToFinalizedMoveStep()
        {
            return node switch
            {
                RootNode => null,
                CharNode charNode => new StepDiff.InsertTextDiff(
                    ParentId: node.ParentElement!.Id,
                    Offset: node.ParentElement?.FindAbsolutTextOffset(charNode) ?? 0, 
                    Text: charNode.Value.ToString()),
                TextNode textNode => new StepDiff.InsertTextDiff(
                    ParentId: node.ParentElement!.Id, 
                    Offset: node.ParentElement?.FindAbsolutTextOffset(textNode) ?? 0, 
                    Text: textNode.TextContent),
                IInlineElement => new StepDiff.MoveInlineDiff(
                    TargetNodeId: node.Id, 
                    TargetParentId: node.ParentElement!.Id,
                    TargetOffset: node.ParentElement?.FindAbsolutTextOffset<IInlineElement>(node) ?? 0),
                IBlockElement => new StepDiff.MoveBlockDiff(
                    TargetNodeId: node.Id,
                    TargetParentId: node.ParentElement!.Id,
                    PreviousSiblingId: node.OriginElement?.Id),
                _ => throw new ArgumentException("Node type is not an element.")
            };
        }
    }
}