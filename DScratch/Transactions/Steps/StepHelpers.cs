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
                CharNode charNode => [new StepDiff.InsertTextDiff(path.Path, GetAbsolutTextOffsetOrDefault(node), charNode.Value.ToString())],
                TextNode textNode => [new StepDiff.InsertTextDiff(path.Path, GetAbsolutTextOffsetOrDefault(node), textNode.TextContent)],
                IInlineElement element => 
                [
                    new StepDiff.InsertElementInlineDiff(path.Path, GetAbsolutTextOffsetOrDefault(node), element.TagName, node.Id),
                    ..node.ChildNodes.SelectMany(c => c.ToInsertSteps())
                ],
                IBlockElement element => 
                [
                    new StepDiff.InsertElementBlockDiff(path.Path, node.Origin?.GetElementPath().Path, element.TagName, node.Id),
                    ..node.ChildNodes.SelectMany(c => c.ToInsertSteps())
                ],
                _ => throw new ArgumentException("Node type is not an element or char.")
            };
        }
        
        public StepDiff ToDeleteSteps()
        {
            var path = node.GetElementPath();
            return node switch
            {
                CharNode => new StepDiff.DeleteTextDiff(path.Path, GetAbsolutTextOffsetOrDefault(node), 1),
                TextNode textNode => new StepDiff.DeleteTextDiff(path.Path, GetAbsolutTextOffsetOrDefault(node), textNode.Length),
                IElement => new StepDiff.DeleteElementDiff(path.Path),
                _ => throw new ArgumentException("Node type is not an element, text or char.")
            };
        }
    }

    private static int GetAbsolutTextOffsetOrDefault(DNode node) => node.Parent?.GetAbsolutTextOffset(node) ?? 0;
}