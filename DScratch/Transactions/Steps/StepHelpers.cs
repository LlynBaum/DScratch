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
            var path = node.Parent?.GetElementPath() ?? NodePath.Root;
            
            return node switch
            {
                CharNode charNode => [new StepDiff.InsertTextDiff(path.Path, node.ParentElement?.FindAbsolutTextOffset(charNode) ?? 0, charNode.Value.ToString())],
                TextNode textNode => [new StepDiff.InsertTextDiff(path.Path, node.ParentElement?.FindAbsolutTextOffset(textNode) ?? 0, textNode.TextContent)],
                IInlineElement element => 
                [
                    new StepDiff.InsertElementInlineDiff(path.Path, node.ParentElement?.FindAbsolutTextOffset<IInlineElement>(node) ?? 0, element.TagName, node.Id),
                    ..node.ChildNodes.SelectMany(c => c.ToInsertSteps())
                ],
                IBlockElement element => 
                [
                    new StepDiff.InsertElementBlockDiff(path.Path, node.OriginElement?.GetElementPath().Path, element.TagName, node.Id),
                    ..node.ChildNodes.SelectMany(c => c.ToInsertSteps())
                ],
                _ => throw new ArgumentException("Node type is not an element, text or char node.")
            };
        }
        
        public StepDiff ToDeleteSteps()
        {
            var path = node.GetElementPath();
            return node switch
            {
                CharNode charNode => new StepDiff.DeleteTextDiff(path.Path, node.ParentElement?.FindAbsolutTextOffset(charNode) ?? 0, 1),
                TextNode textNode => new StepDiff.DeleteTextDiff(path.Path, node.ParentElement?.FindAbsolutTextOffset(textNode) ?? 0, textNode.Length),
                IElement => new StepDiff.DeleteElementDiff(path.Path),
                _ => throw new ArgumentException("Node type is not an element, text or char.")
            };
        }

        public StepDiff?[] ToMoveStep(Action<DNode> move)
        {
            var result = new StepDiff?[2];
            result[0] = node.ToMovePrepStep();
            var oldPath = node.GetElementPath();
            move.Invoke(node);
            result[1] = node.ToFinalizedMoveStep(oldPath);
            return result;
        }

        private StepDiff.DeleteTextDiff? ToMovePrepStep()
        {
            var path = node.GetElementPath();

            return node switch
            {
                CharNode charNode => new StepDiff.DeleteTextDiff(
                    Parent: path.Path, 
                    Offset: node.ParentElement?.FindAbsolutTextOffset(charNode) ?? 0, 
                    Length: 1),
                TextNode textNode => new StepDiff.DeleteTextDiff(
                    Parent: path.Path, 
                    Offset: node.ParentElement?.FindAbsolutTextOffset(textNode) ?? 0, 
                    Length: textNode.Length),
                _ => null
            };
        }
        
        private StepDiff ToFinalizedMoveStep(NodePath oldPath)
        {
            var path = node.GetElementPath();

            return node switch
            {
                CharNode charNode => new StepDiff.InsertTextDiff(
                    Parent: path.Path,
                    Offset: node.ParentElement?.FindAbsolutTextOffset(charNode) ?? 0, 
                    Text: charNode.Value.ToString()),
                TextNode textNode => new StepDiff.InsertTextDiff(
                    Parent: path.Path, 
                    Offset: node.ParentElement?.FindAbsolutTextOffset(textNode) ?? 0, 
                    Text: textNode.TextContent),
                IInlineElement => new StepDiff.MoveInlineDiff(
                    TargetNodePath: oldPath.Path, 
                    TargetParentPath: node.ParentElement!.GetElementPath().Path,
                    TargetOffset: node.ParentElement?.FindAbsolutTextOffset<IInlineElement>(node) ?? 0),
                IBlockElement => new StepDiff.MoveBlockDiff(
                    TargetNodePath: oldPath.Path,
                    TargetParentPath: node.ParentElement!.GetElementPath().Path,
                    PreviousSibling: node.OriginElement?.GetElementPath().Path),
                _ => throw new ArgumentException("Node type is not an element.")
            };
        }
    }
}