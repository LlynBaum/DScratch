using DScratch.Nodes;
using DScratch.Nodes.NodeTypes;

namespace DScratch.Transactions.Steps;

public static class StepHelpers
{
    extension(DNode node)
    {
        public StepDiff ToInsert(NodePath path, int offset)
        {
            if (node is CharNode charNode)
            {
                return new StepDiff.InsertTextDiff(path.Path, offset, charNode.Value.ToString());
            }

            if (node is IElement element)
            {
                return new StepDiff.InsertElementDiff(path.Path, offset, element.TagName, node.Id);
            }

            throw new ArgumentException("Node type is not an element or char.");
        }
        
        public StepDiff ToDelete(NodePath path, int offset)
        {
            if (node is CharNode)
            {
                return new StepDiff.DeleteTextDiff(path.Path, offset, 1);
            }

            return new StepDiff.DeleteElementDiff(path.Path);
        }
    } 
}