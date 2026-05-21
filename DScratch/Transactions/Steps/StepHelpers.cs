using DScratch.Nodes;
using DScratch.Nodes.NodeTypes;

namespace DScratch.Transactions.Steps;

internal static class StepHelpers
{
    extension(DNode node)
    {
        public StepDiff[] ToInsert(NodePath path, int offset)
        {
            return node switch
            {
                CharNode charNode => [new StepDiff.InsertTextDiff(path.Path, offset, charNode.Value.ToString())],
                IElement element and IText text =>
                [
                    new StepDiff.InsertElementDiff(path.Path, offset, element.TagName, node.Id),
                    new StepDiff.InsertTextDiff(path.Path, offset, text.TextContent) // TODO: test with js, should work in theory but not sure
                ],
                IElement element => [new StepDiff.InsertElementDiff(path.Path, offset, element.TagName, node.Id)],
                _ => throw new ArgumentException("Node type is not an element or char.")
            };
        }
        
        public StepDiff ToDelete(NodePath path, int offset)
        {
            return node switch
            {
                CharNode => new StepDiff.DeleteTextDiff(path.Path, offset, 1),
                TextNode textNode => new StepDiff.DeleteTextDiff(path.Path, offset, textNode.Length),
                _ => new StepDiff.DeleteElementDiff(path.Path)
            };
        }
    }
}