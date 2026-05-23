using DScratch.Nodes;
using DScratch.Nodes.NodeTypes;

namespace DScratch.Transactions.Steps;

internal static class StepHelpers
{
    extension(DNode node)
    {
        public StepDiff[] ToInsert()
        {
            // We can not start the path at this node, since it is not in the DOM. So we take the parent node as first possible element
            var path = node.Parent?.GetElementPath() ?? NodePath.Root;
            var offset = node.Parent?.IndexOf(node) ?? 0; // TODO: always offset? I mean would that work, when i just pass 0, that I can insert after element without having text there?
            
            return node switch
            {
                CharNode charNode => [new StepDiff.InsertTextDiff(path.Path, offset, charNode.Value.ToString())],
                TextNode textNode => [new StepDiff.InsertTextDiff(path.Path, offset, textNode.TextContent)],
                IElement element => 
                [
                    new StepDiff.InsertElementDiff(path.Path, offset, element.TagName, node.Id), 
                    ..node.ChildNodes.SelectMany(c => c.ToInsert())
                ],
                _ => throw new ArgumentException("Node type is not an element or char.")
            };
        }
        
        public StepDiff ToDelete()
        {
            var path = node.GetElementPath();
            return node switch
            {
                CharNode => new StepDiff.DeleteTextDiff(path.Path, node.Parent!.IndexOf(node), 1), // TODO: calc absolut offset, JS can not handle relative offsets
                TextNode textNode => new StepDiff.DeleteTextDiff(path.Path, node.Parent!.IndexOf(node), textNode.Length),
                IElement => new StepDiff.DeleteElementDiff(path.Path),
                _ => throw new ArgumentException("Node type is not an element, text or char.")
            };
        }
    }
}